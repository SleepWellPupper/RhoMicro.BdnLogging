// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

using BenchmarkDotNet.Loggers;

internal sealed class WarningState(BenchmarkState? benchmarkState)
    : LogKindDependentState<WarningState>(LogKind.Warning, benchmarkState)
{
    public WarningState() : this(benchmarkState: null)
    {
    }

    public override ConsoleColor Color => ConsoleColor.DarkYellow;
}
