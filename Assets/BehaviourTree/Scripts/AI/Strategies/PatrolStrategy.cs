// Source: https://github.com/adammyhre/Unity-Behaviour-Trees/blob/master/Assets/_Project/Scripts/BehaviourTrees/Strategies.cs
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolStrategy : IStrategy {
    private Transform _entity;
    private NavMeshAgent _agent;
    private List<Transform> _patrolPoints;
    private float _patrolSpeed;
    private int _startIndex = 0;
    private int _currentIndex;
    private bool _isPathCalculated;

    public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, float patrolSpeed = 2.0f) {
        _entity = entity;
        _agent = agent;
        _patrolPoints = patrolPoints;
        _patrolSpeed = patrolSpeed;
    }

    public TaskStatus Process() {
        Transform target = _patrolPoints[_currentIndex];
        _agent.SetDestination(target.position);
        _entity.LookAt(new Vector3(target.position.x, _entity.position.y, target.position.z));
        
        if (_isPathCalculated && _agent.remainingDistance < 0.1f) {
            _currentIndex++;
            Debug.Log(_currentIndex);

            if (_currentIndex == _startIndex) return TaskStatus.Success;

            if (_currentIndex == _patrolPoints.Count)
                _currentIndex = 0;
            _isPathCalculated = false;
        }
        
        // Problem, pathPending is unreliable when too close to destination
        if (_agent.pathPending) {
            _isPathCalculated = true;
        }
        
        return TaskStatus.Running;
    }
    
    public void OnEnter()
    {
        // Get closest patrolPoint
        Vector3 entityPosition = _entity.transform.position;

        _startIndex = 0;
        float shortestDistance = Mathf.Infinity;
        for (int i = 0; i < _patrolPoints.Count; i++)
        {
            float distance = Vector3.Distance(entityPosition, _patrolPoints[i].position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                _startIndex = i;
            }
        }

        _currentIndex = _startIndex;

        _agent.speed = _patrolSpeed;
        _isPathCalculated = false;
    }

    public void Reset() => _currentIndex = 0;
}
