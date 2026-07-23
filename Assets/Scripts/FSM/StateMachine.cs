// Mesin FSM generic, tidak tergantung tipe musuh tertentu.
// Bisa dipakai ulang untuk musuh lain atau Boss kalau nanti mau dirapikan juga.
public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }
}
