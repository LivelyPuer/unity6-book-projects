using UnityEngine;

// Представление и «склейка» с миром Unity
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int capacity = 20;
    public Inventory Inventory { get; private set; }

    void Awake() => Inventory = new Inventory(capacity);
}
