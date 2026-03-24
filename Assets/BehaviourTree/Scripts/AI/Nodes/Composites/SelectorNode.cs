// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Node.cs
public class SelectorNode : CompositeNode
{
    public SelectorNode(string name, params BaseNode[] children) : base(name, children) { }

    protected override TaskStatus Update()
    {
        if (currentChild < children.Count) {
            switch (children[currentChild].Process()) {
                case TaskStatus.Running:
                    return TaskStatus.Running;
                case TaskStatus.Success:
                    Reset();
                    return TaskStatus.Success;
                default:
                    currentChild++;
                    return TaskStatus.Running;
            }
        }
        
        Reset();
        return TaskStatus.Failed;
    }

    protected override void OnEnter() => currentChild = 0;
    protected override void OnExit() => currentChild = 0;
}