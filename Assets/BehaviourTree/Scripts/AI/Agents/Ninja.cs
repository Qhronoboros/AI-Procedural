using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ninja : MonoBehaviour
{
    private Blackboard _blackboard;
    private CompositeNode _tree;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _blackboard = BlackboardManager.globalBlackboard;

        _tree = new SequenceNode("Tree");

        // _tree.AddChild(new TaskNode("CheckDistance", new ConditionStrategy(() => Vector3.Distance(_blackboard.GetVariable<Vector3>(VariableNames.PLAYER_POSITION), transform.position) < 5.0f)));

        _tree.SetupBlackboard(_blackboard);
    }

    private void FixedUpdate()
    {
        // TaskStatus result = _tree.Process();

        // switch (result)
        // {
        //     case TaskStatus.Success: Debug.Log("Success"); break;
        //     case TaskStatus.Failed: Debug.Log("Failed"); break;
        //     case TaskStatus.Running: Debug.Log("Running"); break;
        // }
    }
}
