// Source https://github.com/vmuijrers/GameDevAI/blob/main/BehaviourTreeExample/Assets/Scripts/Player/Player.cs
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _Camera;
    [SerializeField] private float _rotationSpeed = 180.0f;
    [SerializeField] private float _moveSpeed = 3.0f;
    private Rigidbody _rb;
    private Collider _mainCollider;
    private float _inputVertical = 0.0f;
    private float _inputHorizontal = 0.0f;

    private int _maxHealth = 10;
    private int _health;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCollider = GetComponent<Collider>();
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rib in rigidBodies)
        {
            rib.isKinematic = true;
            rib.useGravity = false;
        }

        _mainCollider.enabled = true;
        _rb.isKinematic = false;

        _health = _maxHealth;
    }

    private void Start()
    {
        BlackboardManager.globalBlackboard.SetVariable(VariableNames.PLAYER_POSITION, transform.position);
        BlackboardManager.globalBlackboard.SetVariable(VariableNames.PLAYER_DEAD, false);
    }

    private void FixedUpdate()
    {
        _inputVertical = Input.GetAxis("Vertical");
        _inputHorizontal = Input.GetAxis("Horizontal");
        
        Vector3 forwardDirection = Vector3.Scale(new Vector3(1, 0, 1), _Camera.transform.forward);
        Vector3 rightDirection = Vector3.Cross(Vector3.up, forwardDirection.normalized);
        Vector3 moveDirection = forwardDirection.normalized * _inputVertical + rightDirection.normalized * _inputHorizontal;
        
        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection.normalized, Vector3.up), _rotationSpeed * Time.deltaTime);
        
        transform.position += moveDirection.normalized * _moveSpeed * Time.deltaTime;

        BlackboardManager.globalBlackboard.SetVariable(VariableNames.PLAYER_POSITION, transform.position);
    }

    public void TakeDamage(int damage)
    {
        _health = Mathf.Max(_health - damage, 0);

        if (_health <= 0)
        {
            Debug.Log("Player Dead");
            BlackboardManager.globalBlackboard.SetVariable(VariableNames.PLAYER_DEAD, true);
            Destroy(gameObject);
        }
    }
}