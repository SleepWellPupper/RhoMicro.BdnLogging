// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Loggers;


internal sealed class ErrorState(BenchmarkState? benchmarkState)
    : LogKindDependentState<ErrorState>(LogKind.Error, benchmarkState)
{
    public ErrorState() : this(benchmarkState: null)
    {
    }

    public override ConsoleColor Color => ConsoleColor.DarkRed;
}
