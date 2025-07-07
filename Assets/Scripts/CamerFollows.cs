using Unity.Burst.Intrinsics;
using UnityEngine;

public class CameraFollows : MonoBehaviour
{
    public Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if (target != null)
            transform.position = new Vector3(target.position.x, target.position.y, -10);
    }
}

