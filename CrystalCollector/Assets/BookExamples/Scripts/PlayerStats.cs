#pragma warning disable CS0414 // поля нужны только для демонстрации Inspector
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Movement")]
    [Range(0f, 20f)] [SerializeField] private float moveSpeed = 6f;
    [Tooltip("Высота прыжка в метрах")]
    [SerializeField] private float jumpHeight = 2.5f;

    [Space(10)]
    [Header("Combat")]
    [Min(1)] [SerializeField] private int maxHealth = 100;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Color teamColor = Color.blue;

    [TextArea(2, 5)]
    [SerializeField] private string bio;

    [HideInInspector] public int debugCounter;   // public, но скрыто
    // вообще не сериализуется
    [System.NonSerialized] public float runtimeOnly;
}