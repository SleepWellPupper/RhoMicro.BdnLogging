// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging; 

using BenchmarkDotNet.Loggers;

internal sealed class HelpState(BenchmarkState? benchmarkState)
    : LogKindDependentState<HelpState>(LogKind.Help, benchmarkState)
{
    public HelpState() : this(benchmarkState: null)
    {
    }

    public override ConsoleColor Color => ConsoleColor.DarkGreen;
}
