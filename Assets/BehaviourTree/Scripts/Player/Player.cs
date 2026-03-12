// Source https://github.com/vmuijrers/GameDevAI
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Camera;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float moveSpeed = 3;
    private Rigidbody rb;
    private Collider mainCollider;
    private float inputVertical = 0;
    private float inputHorizontal = 0;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rib in rigidBodies)
        {
            rib.isKinematic = true;
            rib.useGravity = false;
        }

        mainCollider.enabled = true;
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
        
        Vector3 forwardDirection = Vector3.Scale(new Vector3(1, 0, 1), Camera.transform.forward);
        Vector3 rightDirection = Vector3.Cross(Vector3.up, forwardDirection.normalized);
        Vector3 moveDirection = forwardDirection.normalized * inputVertical + rightDirection.normalized * inputHorizontal;
        
        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection.normalized, Vector3.up), rotationSpeed * Time.deltaTime);
        
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }
}
