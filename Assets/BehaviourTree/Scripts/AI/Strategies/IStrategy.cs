// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Strategies.cs
public interface IStrategy
{
    TaskStatus Process();
    // ! Blackboard setup could probably be removed
    void SetupBlackboard() { }
    void Reset() { }
    void OnEnter() { }
    void OnExit() { }
}
