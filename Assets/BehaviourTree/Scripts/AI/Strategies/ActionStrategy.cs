// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Strategies.cs
using System;

public class ActionStrategy : IStrategy
{
    private Action _action;

    public ActionStrategy(Action action)
    {
        _action = action;        
    }    

    public TaskStatus Process()
    {
        _action();
        return TaskStatus.Success;
    }
}
