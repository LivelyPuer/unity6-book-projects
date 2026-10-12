using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumables : MonoBehaviour
{
    [SerializeField] private PlayerInventory owner;
    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    [SerializeField] private ItemData potion;

    private Inventory inventory;
    private InputAction useItem;

    void Start()
    {
        inventory = owner.Inventory;
        useItem = InputSystem.actions.FindAction("Player/UseItem");
    }

    void Update()
    {
        if (useItem.WasPressedThisFrame()) TryUsePotion(potion);
    }

    public bool TryUsePotion(ItemData potion)
    {
        if (!health.IsAlive || health.Current >= health.Max) return false;
        if (!inventory.TryRemove(potion)) return false;
        health.Heal(potion.healAmount);
        animator.SetTrigger("Drink");
        return true;
    }
}
