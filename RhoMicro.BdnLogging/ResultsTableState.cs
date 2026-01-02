// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging; 

using BenchmarkDotNet.Loggers;

internal sealed class ResultsTableState(BenchmarkState benchmarkState) : State
{
    public override ConsoleColor Color => ConsoleColor.Cyan;

    private Boolean _afterNewline = true;
    private Boolean _inTable = false;

    public override State TransitionBeforeWrite(LogKind kind, String text)
    {
        var isTableLine = IsTableLine(text);

        if (_inTable)
        {
            if (!_afterNewline || isTableLine)
            {
                return this;
            }
            else
            {
                _inTable = false;
                return GetLiveOrBenchmarkState(kind);
            }
        }

        if (_afterNewline && isTableLine)
        {
            WriteHeader();
            _inTable = true;
            return this;
        }

        _inTable = false;
        return GetLiveOrBenchmarkState(kind);
    }

    public override void WriteLineCore(LogKind kind, String text)
    {
        base.WriteLineCore(kind, text);
        _afterNewline = true;
    }

    public override void WriteLineCore()
    {
        base.WriteLineCore();
        _afterNewline = true;
    }

    public override void WriteCore(LogKind kind, String text)
    {
        base.WriteCore(kind, text);
        _afterNewline = false;
    }

    private static Boolean IsTableLine(String text) => text is ['|', ' ' or '-'];
    private void WriteHeader()
    {
        WriteLineCore();
        WriteLineCoreWithColor(LogKind.Statistic, $"{benchmarkState.Name}:");
    }

    private State GetLiveOrBenchmarkState(LogKind kind) => kind is LogKind.Statistic ? GetLiveState() : benchmarkState;

    private ProgressReportingState GetLiveState() => new(new SingleLineLiveState(benchmarkState), benchmarkState);
}
