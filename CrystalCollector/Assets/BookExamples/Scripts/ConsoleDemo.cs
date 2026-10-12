using UnityEngine;

// Пример для окна Console (глава 2): сообщение, предупреждение и ошибка
public class ConsoleDemo : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Start()
    {
        Debug.Log("Уровень Level01 загружен");
        Debug.Log("Кристаллов на уровне: 12");
        Debug.LogWarning("Crystal (7): нет коллайдера — его нельзя собрать");
        FollowTarget();
    }

    private void FollowTarget()
    {
        // target не назначен в Inspector — Unity сообщит об ошибке
        transform.LookAt(target.position);
    }
}
