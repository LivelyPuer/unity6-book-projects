using System;
using System.Collections.Generic;

// Логика: ничего не знает о Unity, кроме данных предмета
public class Inventory
{
    private readonly Dictionary<ItemData, int> items = new();
    public int Capacity { get; }
    public event Action Changed;

    public Inventory(int capacity) => Capacity = capacity;

    public IReadOnlyDictionary<ItemData, int> Items => items;

    public bool TryAdd(ItemData item, int count = 1)
    {
        if (!items.ContainsKey(item) && items.Count >= Capacity)
            return false;
        items.TryGetValue(item, out int current);
        items[item] = current + count;
        Changed?.Invoke();
        return true;
    }

    public bool TryRemove(ItemData item, int count = 1)
    {
        if (!items.TryGetValue(item, out int current) || current < count)
            return false;
        if (current == count) items.Remove(item);
        else items[item] = current - count;
        Changed?.Invoke();
        return true;
    }
}
