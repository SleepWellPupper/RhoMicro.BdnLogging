// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

using System.Globalization;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Loggers;

internal sealed partial class ProgressReportingState : State
{
    public ProgressReportingState(State state, BenchmarkState? benchmarkState = null)
    {
        _state = state;
        _benchmarkState = benchmarkState;
    }

    private readonly State _state;
    private readonly BenchmarkState? _benchmarkState;

    private const String _progressPattern =
        @"\/\/ \*\* Remained ([0-9]+) \(([0-9]+\.[0-9]+)%\).*\(([0-9]+h [0-9]+m) from now\) \*\*";
    
#if NET10_0_OR_GREATER
    [GeneratedRegex(_progressPattern)]
    private static partial Regex ProgressPattern { get; }
#else
    private static Regex ProgressPattern { get; } =
 new(_progressPattern, RegexOptions.Compiled);
#endif

    private void TrySetProgress(LogKind kind, String text)
    {
        if (_benchmarkState is null)
        {
            return;
        }

        if (kind is not LogKind.Header)
        {
            return;
        }

        if (ProgressPattern.Match(text).Groups is not
            [
                _,
                { Value: { } countValue },
                { Value: { } percentageValue },
                { Value: { } eta },
            ])
        {
            return;
        }

        if (!Int32.TryParse(countValue, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        _benchmarkState.RemainingCount = count;
        _benchmarkState.Eta = eta;
    }

    public override State TransitionAndWriteLine(LogKind kind, String text)
    {
        TrySetProgress(kind, text);
        return _state.TransitionAndWriteLine(kind, text);
    }

    public override State TransitionAndWriteLine() => _state.TransitionAndWriteLine();

    public override State TransitionAndWrite(LogKind kind, String text)
    {
        TrySetProgress(kind, text);
        return _state.TransitionAndWrite(kind, text);
    }

    public override State TransitionAfterWriteLine(LogKind kind, string text)
    {
        TrySetProgress(kind, text);
        return _state.TransitionAfterWriteLine(kind, text);
    }

    public override State TransitionAfterWriteLine() => _state.TransitionAfterWriteLine();

    public override State TransitionBeforeWriteLine(LogKind kind, string text)
    {
        TrySetProgress(kind, text);

        return _benchmarkState?.TryAdvance(text) ?? false
            ? _benchmarkState
            : _state.TransitionBeforeWriteLine(kind, text);
    }

    public override State TransitionBeforeWriteLine() => _state.TransitionBeforeWriteLine();

    public override void WriteLineCore(LogKind kind, string text)
    {
        TrySetProgress(kind, text);
        _state.WriteLineCore(kind, text);
    }

    public override void WriteLineCore() => _state.WriteLineCore();

    public override void WriteCore(LogKind kind, string text)
    {
        TrySetProgress(kind, text);
        _state.WriteCore(kind, text);
    }

    public override State TransitionAfterWrite(LogKind kind, string text)
    {
        TrySetProgress(kind, text);
        return _state.TransitionAfterWrite(kind, text);
    }

    public override State TransitionBeforeWrite(LogKind kind, string text)
    {
        TrySetProgress(kind, text);

        return _benchmarkState?.TryAdvance(text) ?? false
            ? _benchmarkState
            : _state.TransitionBeforeWrite(kind, text);
    }

    protected override State GetIdentity() => _state;
}
