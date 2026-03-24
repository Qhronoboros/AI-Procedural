// Always returns Success, except if Running
public class OptionalNode : DecoratorNode
{
    public OptionalNode(string name, BaseNode child) : base(name, child) { }

    protected override TaskStatus Update()
    {
        if (child.Process() == TaskStatus.Running)
            return TaskStatus.Running;

        return TaskStatus.Success;
    }
}