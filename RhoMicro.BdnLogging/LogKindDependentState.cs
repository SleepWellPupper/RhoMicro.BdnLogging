// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging; 

using BenchmarkDotNet.Loggers;

internal abstract class LogKindDependentState(
    LogKind requiredKind,
    BenchmarkState? benchmarkState = null)
    : State
{
    protected BenchmarkState? BenchmarkState => benchmarkState;
    private LogKind RequiredKind => requiredKind;

    public override State TransitionBeforeWrite(LogKind kind, String text)
        => kind == RequiredKind ? this : GetDefaultWriteState(kind, text);

    public override State TransitionBeforeWriteLine(LogKind kind, String text)
        => kind == RequiredKind ? this : GetDefaultWriteLineState(kind, text);

    public override State TransitionBeforeWriteLine() => this;

    protected virtual State GetDefaultWriteLineState(LogKind kind, String text)
        => (State?)BenchmarkState ?? DefaultState.Instance;

    protected virtual State GetDefaultWriteState(LogKind kind, String text)
        => (State?)BenchmarkState ?? DefaultState.Instance;
}

internal abstract class LogKindDependentState<TSelf>(
    LogKind requiredKind,
    BenchmarkState? benchmarkState = null)
    : LogKindDependentState(requiredKind, benchmarkState)
    where TSelf : LogKindDependentState<TSelf>, new()
{
    public static TSelf Instance { get; } = new();
}
