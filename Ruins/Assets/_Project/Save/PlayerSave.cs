using System;
using UnityEngine;

// Состояние героя в сохранении: позиция, поворот, предметы
public class PlayerSave : MonoBehaviour, ISaveable
{
    [Serializable]
    private struct State
    {
        public Vector3 position;
        public float yaw;
        public string[] itemIds;
        public int[] counts;
    }

    [SerializeField] private PlayerMotor motor;
    [SerializeField] private PlayerInventory owner;
    [SerializeField] private ItemData[] knownItems;   // для поиска по id

    public string CaptureJson()
    {
        var items = owner.Inventory.Items;
        var s = new State
        {
            position = transform.position,
            yaw = transform.eulerAngles.y,
            itemIds = new string[items.Count],
            counts = new int[items.Count],
        };
        int i = 0;
        foreach (var (item, count) in items)
        {
            s.itemIds[i] = item.id;
            s.counts[i++] = count;
        }
        return JsonUtility.ToJson(s);
    }

    public void RestoreJson(string json)
    {
        State s = JsonUtility.FromJson<State>(json);
        motor.Teleport(s.position, Quaternion.Euler(0f, s.yaw, 0f));
        for (int i = 0; i < s.itemIds.Length; i++)
        {
            ItemData item = Array.Find(knownItems, x => x.id == s.itemIds[i]);
            if (item != null) owner.Inventory.TryAdd(item, s.counts[i]);
        }
    }
}
