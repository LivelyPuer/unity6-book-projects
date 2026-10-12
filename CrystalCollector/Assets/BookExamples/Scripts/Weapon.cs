#pragma warning disable CS0414 // поля нужны только для демонстрации Inspector
using UnityEngine;

// Заглушка оружия для примера с атрибутами (глава 6)
public class Weapon : MonoBehaviour
{
    [SerializeField] private int damage = 10;
}
