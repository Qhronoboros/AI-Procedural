using UnityEngine;

public class CameraFacer : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Camera.main)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
