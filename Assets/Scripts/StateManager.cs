
public enum State
{
    Map,
    Camera
}

public delegate void OnStateChangeHandler();

public class StateManager
{
    private static StateManager _instance = null;
    public event OnStateChangeHandler OnStateChange;
    public State state { get; private set; }
    protected StateManager() {}

    public static StateManager Instance
    {
        get
        {
            if (StateManager._instance == null)
                StateManager._instance = new StateManager();
            return StateManager._instance;
        }
    }

    public void SetState(State state)
    {
        this.state = state;

        if (OnStateChange != null)
            OnStateChange();
    }
}
