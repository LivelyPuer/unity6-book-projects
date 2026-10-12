using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new(0f, 7f, -9f);
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private float lookHeight = 0.5f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position, desired, ref velocity, smoothTime);
        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}