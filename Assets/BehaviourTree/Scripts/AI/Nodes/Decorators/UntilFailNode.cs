// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Node.cs

// Repeat until return Failed
public class UntilFailNode : DecoratorNode
{
    public UntilFailNode(string name, BaseNode child) : base(name, child) { }

    protected override TaskStatus Update()
    {
        if (child.Process() == TaskStatus.Failed)
        {
            Reset();
            return TaskStatus.Failed;
        }

        return TaskStatus.Running;
    }
}