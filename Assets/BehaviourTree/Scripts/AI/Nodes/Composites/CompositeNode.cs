using System.Collections.Generic;
using System.Linq;

public class CompositeNode : BaseNode
{
    protected List<BaseNode> children = new();
    protected int currentChild;

    public CompositeNode(string name, params BaseNode[] children) : base(name)
    {
        this.children = children.ToList();
    }

    public override string GetName() => children[currentChild].GetName();
    public void AddChild(BaseNode child) => children.Add(child);

    public override void SetupBlackboard(Blackboard blackboard)
    {
        this.blackboard = blackboard;
        foreach (BaseNode child in children)
        {
            child.SetupBlackboard(blackboard);
        }
    }

    protected override TaskStatus Update() => children[currentChild].Process();
    public override void Reset()
    {
        currentChild = 0;
        foreach (BaseNode child in children)
        {
            child.Reset();
        }
    }
}