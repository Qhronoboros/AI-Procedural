// Source: https://github.com/vmuijrers/GameDevAI/blob/main/BehaviourTreeExample/Assets/Scripts/AI/BTNodes/BTComposites/BTSequence.cs
public class SequenceNode : CompositeNode
{
    public SequenceNode(string name, params BaseNode[] children) : base(name, children) { }

    protected override TaskStatus Update()
    {
        for( ; currentChild < children.Count; currentChild++)
        {
            var result = children[currentChild].Process();
            switch (result)
            {
                case TaskStatus.Success: continue;
                case TaskStatus.Failed: return TaskStatus.Failed;
                case TaskStatus.Running: return TaskStatus.Running;
            }
        }

        Reset();
        return TaskStatus.Success;
    }

    protected override void OnEnter() => currentChild = 0;
    protected override void OnExit() => currentChild = 0;
}