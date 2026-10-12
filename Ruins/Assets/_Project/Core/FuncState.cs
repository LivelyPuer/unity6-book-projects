using System;

public class FuncState : IState
{
    private readonly Action enter, exit;
    private readonly Action<float> tick;

    public FuncState(Action enter = null, Action<float> tick = null,
                     Action exit = null)
    {
        this.enter = enter;
        this.tick = tick;
        this.exit = exit;
    }

    public void Enter() => enter?.Invoke();
    public void Tick(float dt) => tick?.Invoke(dt);
    public void Exit() => exit?.Invoke();
}
