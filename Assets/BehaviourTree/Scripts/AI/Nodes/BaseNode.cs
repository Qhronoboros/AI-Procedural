public abstract class BaseNode
{
    private bool _wasEntered = false;
    protected Blackboard blackboard;
    public string name;

    public BaseNode(string name = "Node")
    {
        this.name = name;
    }

    public virtual void SetupBlackboard(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public virtual void Reset() { }

    public TaskStatus Process()
    {
        if (!_wasEntered)
        {
            OnEnter();
            _wasEntered = true;
        }

        var result = Update();
        if (result != TaskStatus.Running)
        {
            OnExit();
            _wasEntered = false;
        }
        return result;
    }

    protected abstract TaskStatus Update();
    protected virtual void OnEnter() { }
    protected virtual void OnExit() { }
}
