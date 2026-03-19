using UnityEngine;

public class BlackboardManager : MonoBehaviour
{
	public static BlackboardManager instance { get; private set;}

    // ! Maybe have a dictionary for key = object type, value = blackboard
    public static Blackboard globalBlackboard { get; private set;}

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
		{
			Debug.LogError($"A BlackboardManager already exists, deleting self: {name}");
			Destroy(gameObject);
            return;
		}

        globalBlackboard = new Blackboard();
	}
}
