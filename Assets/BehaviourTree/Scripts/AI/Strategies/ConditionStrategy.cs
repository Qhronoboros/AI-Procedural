// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Strategies.cs
using System;

public class ConditionStrategy : IStrategy
{
    private Func<bool> _predicate; 

    public ConditionStrategy(Func<bool> predicate)
    {
        _predicate = predicate;
    }

    public TaskStatus Process() => _predicate() ? TaskStatus.Success : TaskStatus.Failed;
}
