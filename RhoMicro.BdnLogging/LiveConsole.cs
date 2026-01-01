// SPDX-License-Identifier: MPL-2.0

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal class LiveConsole
{
    public static LiveConsole Default { get; } = new();

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
        else
        {
            top -= 2;
        }

        _liveTop = top;

        var messages = message.Split(
                ['\n', '\r'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(m => m[..(m.Length > bufferWidth ? bufferWidth : m.Length)]);

        foreach (var m in messages)
        {
            Console.SetCursorPosition(0, _liveTop);

            WriteProgress(progress, bufferWidth);
            Console.Write(m);
            WritePadding(m.Length, bufferWidth);
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
            var paddingWidth = (paddedLabelWidth - label.Length) / 2f;
            var leftPaddingWidth = (Int32)Math.Floor(paddingWidth);
            remaining[..leftPaddingWidth].Fill(' ');
            remaining = remaining[leftPaddingWidth..];
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

            var primaryColor = progress.PrimaryColor ?? previousForegroundColor;
            var secondaryColor = progress.SecondaryColor ?? previousBackgroundColor;

            Console.ForegroundColor = secondaryColor;
            Console.BackgroundColor = primaryColor;
            Console.Write(leftPart);

            Console.ForegroundColor = primaryColor;
            Console.BackgroundColor = secondaryColor;
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
        Console.SetCursorPosition(0, _liveTop + 1);
        WritePadding(0, bufferWidth);
        Console.SetCursorPosition(0, _liveTop);
        _isLive = false;
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

        // _liveTop += 2;
        // Console.WriteLine();
        // Console.WriteLine();
        
        Console.WriteLine();
    }
}
