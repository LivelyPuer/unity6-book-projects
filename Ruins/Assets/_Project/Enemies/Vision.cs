using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField] private Transform eye;
    [SerializeField] private float range = 14f;
    [SerializeField] private float angle = 110f;
    [SerializeField] private LayerMask obstacles;

    public bool CanSee(Transform target)
    {
        Vector3 to = target.position + Vector3.up * 1.2f - eye.position;
        if (to.sqrMagnitude > range * range) return false;
        if (Vector3.Angle(eye.forward, to) > angle * 0.5f) return false;
        return !Physics.Raycast(eye.position, to.normalized, to.magnitude,
                                obstacles, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        if (eye == null) return;
        Gizmos.color = Color.yellow;
        Vector3 l = Quaternion.Euler(0, -angle / 2, 0) * eye.forward * range;
        Vector3 r = Quaternion.Euler(0, angle / 2, 0) * eye.forward * range;
        Gizmos.DrawLine(eye.position, eye.position + l);
        Gizmos.DrawLine(eye.position, eye.position + r);
    }
}
