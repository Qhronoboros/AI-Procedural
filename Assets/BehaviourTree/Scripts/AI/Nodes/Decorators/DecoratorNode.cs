public class DecoratorNode : BaseNode
{
    protected BaseNode child;

    public DecoratorNode(string name, BaseNode child) : base(name)
    {
        this.child = child;
    }

    public void SetChild(BaseNode child) => this.child = child;

    public override void SetupBlackboard(Blackboard blackboard)
    {
        this.blackboard = blackboard;
        child.SetupBlackboard(blackboard);
    }

    protected override TaskStatus Update() => child.Process();
    public override void Reset()
    {
        child.Reset();
    }
}