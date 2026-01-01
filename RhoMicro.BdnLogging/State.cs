// SPDX-License-Identifier: MPL-2.0

using BenchmarkDotNet.Loggers;

internal abstract class State : IEquatable<State>
{
    public virtual State TransitionAndWriteLine(LogKind kind, String text)
        => TransitionAndWriteLineCore(kind, text, []);

    private State TransitionAndWriteLineCore(LogKind kind, String text, HashSet<State> transitionedFrom)
        => TransitionAndWrite(
            static (k, t, s, h) => s.TransitionAndWriteLineCore(k, t, h),
            static (k, t, s) => s.TransitionBeforeWriteLine(k, t),
            static (k, t, s) => s.WriteLineCoreWithColor(k, t),
            static (k, t, s) => s.TransitionAfterWriteLine(k, t),
            kind,
            text,
            transitionedFrom);

    public virtual State TransitionAndWriteLine()
        => TransitionAndWriteLineCore([]);

    private State TransitionAndWriteLineCore(HashSet<State> transitionedFrom)
        => TransitionAndWrite(
            static (_, _, s, h) => s.TransitionAndWriteLineCore(h),
            static (_, _, s) => s.TransitionBeforeWriteLine(),
            static (_, _, s) => s.WriteLineCore(),
            static (_, _, s) => s.TransitionAfterWriteLine(),
            LogKind.Default,
            text: String.Empty,
            transitionedFrom);

    public virtual State TransitionAndWrite(LogKind kind, String text)
        => TransitionAndWriteCore(kind, text, []);

    private State TransitionAndWriteCore(LogKind kind, String text, HashSet<State> transitionedFrom)
        => TransitionAndWrite(
            static (k, t, s, h) => s.TransitionAndWriteCore(k, t, h),
            static (k, t, s) => s.TransitionBeforeWrite(k, t),
            static (k, t, s) => s.WriteCoreWithColor(k, t),
            static (k, t, s) => s.TransitionAfterWrite(k, t),
            kind,
            text,
            transitionedFrom);

    protected void WriteLineCoreWithColor(LogKind kind, String text)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = Color;
        WriteLineCore(kind, text);
        Console.ForegroundColor = previousColor;
    }

    public virtual void WriteLineCore(LogKind kind, String text)
    {
        LiveConsole.Default.WriteLine(text);
    }

    public virtual void WriteLineCore()
    {
        LiveConsole.Default.WriteLine();
    }

    protected void WriteCoreWithColor(LogKind kind, String text)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = Color;
        WriteCore(kind, text);
        Console.ForegroundColor = previousColor;
    }

    public virtual void WriteCore(LogKind kind, String text)
    {
        LiveConsole.Default.Write(text);
    }

    public virtual ConsoleColor Color => ConsoleColor.White;

    public virtual State TransitionBeforeWrite(LogKind kind, String text)
        => this;

    public virtual State TransitionBeforeWriteLine(LogKind kind, String text)
        => TransitionBeforeWrite(kind, text);

    public virtual State TransitionBeforeWriteLine()
        => TransitionBeforeWriteLine(LogKind.Default, text: String.Empty);

    public virtual State TransitionAfterWrite(LogKind kind, String text)
        => this;

    public virtual State TransitionAfterWriteLine(LogKind kind, String text)
        => TransitionAfterWrite(kind, text);

    public virtual State TransitionAfterWriteLine()
        => TransitionAfterWriteLine(LogKind.Default, text: String.Empty);

    private State TransitionAndWrite(
        Func<LogKind, String, State, HashSet<State>, State> write,
        Func<LogKind, String, State, State> transitionBeforeWrite,
        Action<LogKind, String, State> writeCore,
        Func<LogKind, String, State, State> transitionAfterWrite,
        LogKind kind,
        String text,
        HashSet<State> transitionedFrom)
    {
        var preWriteState = tryTransition(transitionBeforeWrite, this);
        var wasNotTransitioned = Equals(preWriteState);
        State postWriteState;

        if (wasNotTransitioned)
        {
            writeCore.Invoke(kind, text, preWriteState);
            postWriteState = tryTransition(transitionAfterWrite, preWriteState);
        }
        else
        {
            var intermediatePostWriteState = write.Invoke(kind, text, preWriteState, transitionedFrom);
            postWriteState = tryTransition(transitionAfterWrite, intermediatePostWriteState);
        }

        return postWriteState;

        State tryTransition(Func<LogKind, String, State, State> transition, State state)
        {
            if (transitionedFrom.Contains(state))
            {
                return this;
            }

            var result = state;

            do
            {
                state = result;
                result = transition.Invoke(kind, text, state);
            } while (!state.Equals(result));

            return result;
        }
    }

    protected virtual State GetIdentity() => this;

    public override Boolean Equals(object? obj) => Equals(obj as State);

    public override Int32 GetHashCode()
    {
        var identity = GetIdentity();
        return ReferenceEquals(identity, this) ? base.GetHashCode() : identity.GetHashCode();
    }

    public Boolean Equals(State? other)
    {
        var identity = GetIdentity();
        var otherIdentity = other?.GetIdentity();
        return ReferenceEquals(identity, otherIdentity);
    }
}

internal abstract class State<TSelf> : State
    where TSelf : State<TSelf>, new()
{
    public static TSelf Instance { get; } = new();
}
