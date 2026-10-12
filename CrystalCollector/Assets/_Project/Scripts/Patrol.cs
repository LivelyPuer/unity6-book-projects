using UnityEngine;

[SelectionBase]
public class Patrol : MonoBehaviour
{
    [Header("Маршрут")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool loop = true;

    [Header("Движение")]
    [Min(0f)] [SerializeField] private float speed = 3f;
    [Min(0f)] [SerializeField] private float turnSpeed = 360f; // градусов/с
    [Min(0f)] [SerializeField] private float waitTime = 1f;

    private int index;
    private int direction = 1;
    private float waitTimer;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 target = waypoints[index].position;
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f; // двигаемся в горизонтальной плоскости

        // Поворот к цели
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // Движение к цели без «перелёта»
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(target.x, transform.position.y, target.z),
            speed * Time.deltaTime);

        if (toTarget.magnitude < 0.05f)
            NextWaypoint();
    }

    void NextWaypoint()
    {
        waitTimer = waitTime;
        if (loop)
        {
            index = (index + 1) % waypoints.Length;
        }
        else
        {
            int next = index + direction;
            if (next >= waypoints.Length || next < 0)
                direction = -direction; // туда-обратно
            index += direction;
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            int next = i + 1;
            if (next < waypoints.Length && waypoints[next] != null)
                Gizmos.DrawLine(waypoints[i].position,
                                waypoints[next].position);
            else if (loop && waypoints[0] != null)
                Gizmos.DrawLine(waypoints[i].position,
                                waypoints[0].position);
        }
    }
}