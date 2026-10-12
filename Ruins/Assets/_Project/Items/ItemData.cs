using UnityEngine;

public enum ItemKind { Key, Consumable, Quest }

[CreateAssetMenu(menuName = "Ruins/Item")]
public class ItemData : ScriptableObject
{
    public string id;              // постоянный ключ для сохранений
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemKind kind;
    public int healAmount;         // для зелий
    public int price;              // цена в магазине
}
