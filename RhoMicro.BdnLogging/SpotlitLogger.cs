// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

using BenchmarkDotNet.Loggers;

/// <summary>
/// Provides filtered console logging that only persistently emits hints, statistics tables, warnings and errors.
/// All other logs are only emitted into an ephemeral progress bar. 
/// </summary>
public sealed class SpotlitLogger : ILogger
{
    private SpotlitLogger()
    {
    }

    /// <summary>
    /// Gets the singleton instance of the logger.
    /// </summary>
    public static SpotlitLogger Instance { get; } = new();

    /// <inheritdoc/>
    public String Id => nameof(SpotlitLogger);

    /// <inheritdoc/>
    public Int32 Priority => 0;

    private const String _lastText = "Artifacts cleanup is finished";

    private State _currentState = DefaultState.Instance;

    /// <inheritdoc/>
    public void WriteLine(LogKind logKind, String text)
    {
        if (text is _lastText)
        {
            LiveConsole.Default.ClearLive();
            return;
        }

        _currentState = _currentState.TransitionAndWriteLine(logKind, text);
    }

    /// <inheritdoc/>
    public void Write(LogKind logKind, String text)
    {
        if (text is _lastText)
        {
            LiveConsole.Default.ClearLive();
            return;
        }

        _currentState = _currentState.TransitionAndWrite(logKind, text);
    }

    /// <inheritdoc/>
    public void WriteLine()
        => _currentState = _currentState.TransitionAndWriteLine();

    /// <inheritdoc/>
    public void Flush()
    {
    }
}
