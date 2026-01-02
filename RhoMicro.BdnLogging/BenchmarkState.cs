// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Loggers;

internal sealed partial class BenchmarkState : State
{
    public BenchmarkState(String name, String method, int totalCount)
    {
        _name = name;
        _method = method;
        _totalCount = totalCount;
    }

    private Int32 _totalCount;
    private readonly String _name;
    private String _method;

#if NET10_0_OR_GREATER
    [GeneratedRegex(@"(?:\/\/ Benchmark: )([a-zA-Z0-9]+)(?:\.)([a-zA-Z0-9]+)")]
    private static partial Regex NamePattern { get; }

    [GeneratedRegex(@"(?:\/\/ \*\*\*\*\* Found )([0-9]+)")]
    private static partial Regex FoundPattern { get; }
#else
    private static Regex NamePattern { get; } =
        new(@"(?:\/\/ Benchmark: )([a-zA-Z0-9]+)(?:\.)([a-zA-Z0-9]+)", RegexOptions.Compiled);
    
    private static Regex FoundPattern { get; } 
        = new(@"(?:\/\/ \*\*\*\*\* Found )([0-9]+)", RegexOptions.Compiled);
#endif

    public String Name => _name;
    public String Method => ProgressRatio is 1 ? String.Empty : _method;

    public Single ProgressRatio => TotalCount is not 0
        ? CompletedCount / (Single)TotalCount
        : 0;

    public String Eta { get; set; } = "unknown";
    public Int32? RemainingCount { get; set; }

    public Int32 CompletedCount => RemainingCount is { } remainingCount
        ? TotalCount - remainingCount
        : 0;

    public Int32 TotalCount => _totalCount;

    public Boolean TryAdvance(String text)
    {
        if (text.StartsWith("// ***** BenchmarkRunner: Finish  *****"))
        {
            _method = String.Empty;
            return false;
        }

        if (NamePattern.Match(text).Groups is not [_, { Value: var name }, { Value: var method }]
         || name != _name
         || _method == method)
        {
            return false;
        }

        _method = method;
        return true;
    }

    public static Boolean TryCreate(String text, [NotNullWhen(true)] out BenchmarkState? state)
    {
        if (FoundPattern.Match(text).Groups is [_, { Value: var totalCountValue }]
         && Int32.TryParse(totalCountValue, out var totalCount))
        {
            state = new BenchmarkState(name: String.Empty, method: String.Empty, totalCount);
            return true;
        }

        if (NamePattern.Match(text).Groups is [_, { Value: var name }, { Value: var method }])
        {
            state = new BenchmarkState(name, method, totalCount: 0);
            return true;
        }

        state = null;
        return false;
    }

    public override ConsoleColor Color => ConsoleColor.Gray;

    public override State TransitionBeforeWrite(LogKind kind, String text)
    {
        return kind switch
        {
            LogKind.Help => new HelpState(this),
            LogKind.Warning => new WarningState(this),
            LogKind.Error => new ErrorState(this),
            LogKind.Header when TryCreate(text, out var benchmarkState) && benchmarkState.Name != _name =>
                benchmarkState.WithProgress(RemainingCount, TotalCount, Eta),
            LogKind.Header => new ProgressReportingState(new SingleLineLiveState(this), this),
            LogKind.Statistic => new ResultsTableState(this),
            _ => new ContinuousLiveState(kind, this),
        };
    }

    private BenchmarkState WithProgress(
        Int32? remainingCount,
        Int32 totalCount,
        String eta)
    {
        RemainingCount = remainingCount;
        Eta = eta;
        _totalCount = totalCount;

        return this;
    }
}
