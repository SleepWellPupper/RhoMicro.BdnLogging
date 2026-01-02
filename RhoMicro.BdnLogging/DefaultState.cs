// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging; 

using BenchmarkDotNet.Loggers;

internal sealed class DefaultState : State<DefaultState>
{
    public override ConsoleColor Color => ConsoleColor.White;

    public override void WriteLineCore(LogKind kind, String text) => LiveConsole.Default.WriteLine(text);

    public override void WriteCore(LogKind kind, String text) => LiveConsole.Default.Write(text);

    public override void WriteLineCore() => LiveConsole.Default.WriteLine();

    public override State TransitionBeforeWrite(LogKind kind, String text)
    {
        return kind switch
        {
            LogKind.Help => HelpState.Instance,
            LogKind.Warning => WarningState.Instance,
            LogKind.Error => ErrorState.Instance,
            LogKind.Header when BenchmarkState.TryCreate(text, out var benchmarkState) => benchmarkState,
            LogKind.Header => new SingleLineLiveState(),
            LogKind.Hint
                or LogKind.Default
                or LogKind.Info => new ContinuousLiveState(kind),
            _ => this
        };
    }
}
