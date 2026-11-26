
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target == null) return;

        transform.LookAt(target.position);
    }
}
