// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Loggers;

internal sealed partial class ContinuousLiveState(
    LogKind requiredKind,
    BenchmarkState? benchmarkState = null)
    : LogKindDependentState(requiredKind, benchmarkState)
{
    public override ConsoleColor Color => ConsoleColor.DarkGray;

    public override void WriteCore(LogKind kind, String text) => LiveConsole.Default.WriteLive(text, LiveMessageProgress.Create(BenchmarkState));

    public override void WriteLineCore(LogKind kind, String text) => LiveConsole.Default.WriteLive(text, LiveMessageProgress.Create(BenchmarkState));

    public override void WriteLineCore()
    {
    }
}
