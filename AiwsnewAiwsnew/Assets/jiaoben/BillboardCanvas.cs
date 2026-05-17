using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    public Camera targetCamera;
    void LateUpdate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);
    }
}