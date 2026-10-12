using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable, ISaveable
{
    [SerializeField] private ItemData item;
    [SerializeField] private int count = 1;
    [SerializeField] private SoundData pickupSound;
    private bool taken;

    public string GetPrompt(GameObject actor) => $"Поднять: {item.displayName}";
    public bool CanInteract(GameObject actor) => !taken;

    public void Interact(GameObject actor)
    {
        var inventory = actor.GetComponent<PlayerInventory>().Inventory;
        if (!inventory.TryAdd(item, count)) return;   // нет места
        SoundPlayer.Instance.Play(pickupSound, transform.position);
        SetTaken(true);
    }

    private void SetTaken(bool value)
    {
        taken = value;
        gameObject.SetActive(!value);   // не уничтожаем — нужен для сохранения
    }

    public string CaptureJson() => taken ? "1" : "0";
    public void RestoreJson(string json) => SetTaken(json == "1");
}
