using System;
using System.Collections.Generic;

public interface IState
{
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}

public class StateMachine
{
    private readonly Dictionary<IState, List<Transition>> transitions = new();
    private readonly List<Transition> anyTransitions = new();
    public IState Current { get; private set; }

    private readonly struct Transition
    {
        public readonly IState To;
        public readonly Func<bool> Condition;
        public Transition(IState to, Func<bool> c) { To = to; Condition = c; }
    }

    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        if (!transitions.TryGetValue(from, out var list))
            transitions[from] = list = new List<Transition>();
        list.Add(new Transition(to, condition));
    }

    public void AddAnyTransition(IState to, Func<bool> condition) =>
        anyTransitions.Add(new Transition(to, condition));

    public void SetState(IState state)
    {
        if (state == Current) return;
        Current?.Exit();
        Current = state;
        Current.Enter();
    }

    public void Tick(float deltaTime)
    {
        IState next = FindTransition();
        if (next != null) SetState(next);
        Current?.Tick(deltaTime);
    }

    private IState FindTransition()
    {
        foreach (var t in anyTransitions)
            if (t.To != Current && t.Condition()) return t.To;
        if (Current != null && transitions.TryGetValue(Current, out var list))
            foreach (var t in list)
                if (t.Condition()) return t.To;
        return null;
    }
}
