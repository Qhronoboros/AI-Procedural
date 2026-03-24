public class SelectorNode : CompositeNode
{
    public SelectorNode(string name, params BaseNode[] children) : base(name, children) { }

    protected override TaskStatus Update()
    {
        for( ; currentChild < children.Count; currentChild++)
        {
            var result = children[currentChild].Process();
            switch (result)
            {
                case TaskStatus.Success: return TaskStatus.Success;
                case TaskStatus.Failed: continue;
                case TaskStatus.Running: return TaskStatus.Running;
            }
        }

        Reset();
        return TaskStatus.Failed;
    }

    protected override void OnEnter() => currentChild = 0;
    protected override void OnExit() => currentChild = 0;
}