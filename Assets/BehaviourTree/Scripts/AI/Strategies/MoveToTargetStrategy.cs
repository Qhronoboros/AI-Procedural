// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Strategies.cs
using UnityEngine;
using UnityEngine.AI;

public class MoveToTargetStrategy : IStrategy
{
    private Transform _entity;
    private NavMeshAgent _agent;
    private Transform _target;

    public MoveToTargetStrategy(Transform entity, NavMeshAgent agent, Transform target) {
        _entity = entity;
        _agent = agent;
        _target = target;
    }

    public TaskStatus Process() {
        if (Vector3.Distance(_entity.position, _target.position) < 1f)
            return TaskStatus.Success;
        
        _agent.SetDestination(_target.position);
        _entity.LookAt(new Vector3(_target.position.x, _entity.position.y, _target.position.z));

        return TaskStatus.Running;
    }
}
