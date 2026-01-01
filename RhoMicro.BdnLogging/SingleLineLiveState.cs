// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Loggers;

internal sealed class SingleLineLiveState(BenchmarkState? benchmarkState) : State<SingleLineLiveState>
{
    public SingleLineLiveState() : this(benchmarkState: null)
    {
    }

    public override ConsoleColor Color => ConsoleColor.DarkGray;

    public override void WriteCore(LogKind kind, String text)
        => LiveConsole.Default.WriteLive(text, LiveMessageProgress.Create(benchmarkState));

    public override void WriteLineCore(LogKind kind, String text)
        => LiveConsole.Default.WriteLive(text, LiveMessageProgress.Create(benchmarkState));

    public override void WriteLineCore()
    {
    }

    public override State TransitionAfterWriteLine() => GetDefaultState();
    public override State TransitionAfterWriteLine(LogKind kind, String text) => GetDefaultState();

    private State GetDefaultState() => (State?)benchmarkState ?? DefaultState.Instance;
}
