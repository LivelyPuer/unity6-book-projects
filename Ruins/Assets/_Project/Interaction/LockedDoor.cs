using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable, ISaveable
{
    [SerializeField] private ItemData requiredKey;
    [SerializeField] private Animator animator;
    [SerializeField] private bool consumeKey = true;
    private bool open;

    public string GetPrompt(GameObject actor) =>
        HasKey(actor) ? "Открыть" : $"Нужен: {requiredKey.displayName}";

    public bool CanInteract(GameObject actor) => !open;

    public void Interact(GameObject actor)
    {
        if (!HasKey(actor)) return;   // подсказка уже сообщила о ключе
        if (requiredKey != null && consumeKey)
            Inventory(actor).TryRemove(requiredKey);
        SetOpen(true);
    }

    private static Inventory Inventory(GameObject actor) =>
        actor.GetComponent<PlayerInventory>().Inventory;

    private bool HasKey(GameObject actor) =>
        requiredKey == null
        || Inventory(actor).Items.ContainsKey(requiredKey);

    public void SetOpen(bool value)
    {
        open = value;
        animator.SetBool("Open", value);
    }

    public string CaptureJson() => open ? "1" : "0";
    public void RestoreJson(string json) => SetOpen(json == "1");
}
