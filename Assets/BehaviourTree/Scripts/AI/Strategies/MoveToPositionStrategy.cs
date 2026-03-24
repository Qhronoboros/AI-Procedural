// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Strategies.cs
using UnityEngine;
using UnityEngine.AI;

public class MoveToPositionStrategy : IStrategy
{
    private Transform _entity;
    private NavMeshAgent _agent;
    private Vector3 _position;

    public MoveToPositionStrategy(Transform entity, NavMeshAgent agent, Vector3 position) {
        _entity = entity;
        _agent = agent;
        _position = position;
    }

    public TaskStatus Process() {
        if (Vector3.Distance(_entity.position, _position) < 1f)
            return TaskStatus.Success;
        
        _agent.SetDestination(_position);
        _entity.LookAt(new Vector3(_position.x, _entity.position.y, _position.z));

        return TaskStatus.Running;
    }
}
