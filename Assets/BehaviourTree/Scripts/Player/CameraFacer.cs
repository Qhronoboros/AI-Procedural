using UnityEngine;

public class CameraFacer : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Camera.current)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.current.transform.position);    
    }
}
