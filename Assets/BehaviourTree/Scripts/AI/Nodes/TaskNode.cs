public class TaskNode : BaseNode
{
    private IStrategy _strategy;

    public TaskNode(string name, IStrategy strategy) : base(name)
    {
        _strategy = strategy;
    }

    public override void Reset() => _strategy.Reset();
    protected override TaskStatus Update() => _strategy.Process();
    protected override void OnEnter() => _strategy.OnEnter();
    protected override void OnExit() => _strategy.OnExit();
}
