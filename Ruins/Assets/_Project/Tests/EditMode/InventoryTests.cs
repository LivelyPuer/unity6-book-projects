using NUnit.Framework;
using UnityEngine;

public class InventoryTests
{
    private ItemData potion, key;

    [SetUp]
    public void SetUp()
    {
        potion = ScriptableObject.CreateInstance<ItemData>();
        key = ScriptableObject.CreateInstance<ItemData>();
    }

    [Test]
    public void TryAdd_EmptyInventory_AddsItem()
    {
        var inventory = new Inventory(capacity: 2);

        bool added = inventory.TryAdd(potion, 3);

        Assert.IsTrue(added);
        Assert.AreEqual(3, inventory.Items[potion]);
    }

    [Test]
    public void TryAdd_FullInventory_ReturnsFalse()
    {
        var inventory = new Inventory(capacity: 1);
        inventory.TryAdd(potion);

        Assert.IsFalse(inventory.TryAdd(key));
    }

    [Test]
    public void TryRemove_MoreThanOwned_Fails()
    {
        var inventory = new Inventory(capacity: 1);
        inventory.TryAdd(potion, 1);

        Assert.IsFalse(inventory.TryRemove(potion, 2));
        Assert.AreEqual(1, inventory.Items[potion]);
    }

    [Test]
    public void Changed_IsRaised_OnAdd()
    {
        var inventory = new Inventory(capacity: 1);
        int calls = 0;
        inventory.Changed += () => calls++;

        inventory.TryAdd(potion);

        Assert.AreEqual(1, calls);
    }
}