// SPDX-License-Identifier: MPL-2.0

using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Loggers;

public partial class FocusLogger : ILogger
{
    public static FocusLogger Default { get; } = new();

    public String Id => nameof(FocusLogger);
    public Int32 Priority => 0;

    private const String _lastText = "Artifacts cleanup is finished";

    private State _currentState = DefaultState.Instance;

    public void WriteLine(LogKind logKind, String text)
    {
        if (text is _lastText)
        {
            LiveConsole.Default.ClearLive();
            return;
        }

        _currentState = _currentState.TransitionAndWriteLine(logKind, text);
    }

    public void Write(LogKind logKind, String text)
    {
        if (text is _lastText)
        {
            LiveConsole.Default.ClearLive();
            return;
        }

        _currentState = _currentState.TransitionAndWrite(logKind, text);
    }

    public void WriteLine()
        => _currentState = _currentState.TransitionAndWriteLine();

    public void Flush()
    {
    }
}
