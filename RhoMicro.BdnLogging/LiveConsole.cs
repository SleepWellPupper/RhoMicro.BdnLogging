// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

using System.Buffers;

internal class LiveConsole
{
    public static LiveConsole Default { get; } = new();

    private Boolean _isDirty = false;
    private Boolean _isLive = false;
    private Int32 _liveTop = -1;

    public void WriteLive(String message, LiveMessageProgress progress = default)
    {
        var bufferWidth = Console.BufferWidth;

        var (left, top) = Console.GetCursorPosition();

        if (!_isLive)
        {
            if (left is not 0)
            {
                Console.WriteLine();
                top++;
            }

            _isLive = true;
        }
        else if (_isDirty)
        {
            top -= 2;
        }

        _liveTop = top;

        var messages = message.Split(
            ['\n', '\r'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        const String prefix = "> ";
        var maxLineLength = bufferWidth - prefix.Length;

        foreach (var messageLine in messages)
        {
            Console.SetCursorPosition(0, _liveTop);

            var truncatedLength = messageLine.Length > maxLineLength ? maxLineLength : messageLine.Length;
            var truncatedLine = messageLine.AsSpan(0, truncatedLength);

            WriteProgress(progress, bufferWidth);
            Console.Write(prefix);
            Console.Write(truncatedLine);
            WritePadding(truncatedLength + prefix.Length, bufferWidth);

            _isDirty = true;
        }
    }

    private static void WriteProgress(LiveMessageProgress progress, Int32 bufferWidth)
    {
        const Int32 minBufferWidth = 5;

        if (bufferWidth < minBufferWidth)
        {
            Console.WriteLine();
            return;
        }

        var label = (progress.Name is null or []
            ? String.Empty
            : progress.Name).AsSpan();
        var ellipsis = "...";
        var paddedLabelWidth = bufferWidth - 2; // - brackets
        var maxLabelWidth = paddedLabelWidth - ellipsis.Length; // - ellipsis

        var rentedTextBuffer = ArrayPool<Char>.Shared.Rent(paddedLabelWidth);

        try
        {
            var textBuffer = rentedTextBuffer.AsSpan(0, paddedLabelWidth);

            if (label.Length > maxLabelWidth)
            {
                label = label[..maxLabelWidth];
            }
            else
            {
                ellipsis = String.Empty;
            }

            var remaining = textBuffer;
            label.CopyTo(remaining);
            remaining = remaining[label.Length..];
            ellipsis.CopyTo(remaining);
            remaining = remaining[ellipsis.Length..];
            remaining.Fill(' ');

            var progressWidth = (Int32)Math.Ceiling(textBuffer.Length * progress.ProgressRatio);
            var leftPart = textBuffer[..progressWidth];
            var rightPart = textBuffer[progressWidth..];

            Console.Write("[");

            var previousForegroundColor = Console.ForegroundColor;
            var previousBackgroundColor = Console.BackgroundColor;

            Console.ForegroundColor = progress.CompletedForegroundColor ?? previousBackgroundColor;
            Console.BackgroundColor = progress.CompletedBackgroundColor ?? previousForegroundColor;
            Console.Write(leftPart);

            Console.ForegroundColor = progress.PendingForegroundColor ?? previousForegroundColor;
            Console.BackgroundColor = progress.PendingBackgroundColor ?? previousBackgroundColor;
            Console.Write(rightPart);

            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;

            Console.WriteLine("]");
        }
        finally
        {
            ArrayPool<Char>.Shared.Return(rentedTextBuffer);
        }
    }

    public void WriteLine(String message)
    {
        ClearLive();
        Console.WriteLine(message);
    }

    public void WriteLine()
    {
        ClearLive();
        Console.WriteLine();
    }

    public void Write(String message)
    {
        ClearLive();
        Console.Write(message);
    }

    public void ClearLive()
    {
        if (!_isLive || _liveTop < 0)
        {
            return;
        }

        var bufferWidth = Console.BufferWidth;

        Console.SetCursorPosition(0, _liveTop);
        WritePadding(0, bufferWidth);
        WritePadding(0, bufferWidth);
        Console.SetCursorPosition(0, _liveTop);
        _isLive = false;
        _isDirty = false;
    }

    private void WritePadding(Int32 messageLength, Int32 bufferWidth)
    {
        var paddingWidth = bufferWidth - messageLength;

        if (paddingWidth < 0)
        {
            return;
        }

        var rented = ArrayPool<Char>.Shared.Rent(paddingWidth);
        var padding = rented.AsSpan(0, paddingWidth);
        padding.Fill(' ');
        try
        {
            Console.Write(padding);
        }
        finally
        {
            ArrayPool<Char>.Shared.Return(rented);
        }

#if DEBUG
        // _liveTop += 2;
        // Console.WriteLine();
        // Console.WriteLine();
#endif

        Console.WriteLine();
    }
}

#if !NET10_0_OR_GREATER
file static class ConsoleExtensions
{
    extension(Console)
    {
        public static void Write(ReadOnlySpan<Char> text) => Console.Write(text.ToString());
    }
}
#endif
