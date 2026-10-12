using System.Collections.Generic;
using UnityEngine;

public class AttackTokens : MonoBehaviour
{
    [SerializeField] private int maxAttackers = 2;
    private readonly HashSet<object> holders = new();

    public bool TryAcquire(object attacker)
    {
        if (holders.Contains(attacker)) return true;
        if (holders.Count >= maxAttackers) return false;
        holders.Add(attacker);
        return true;
    }

    public void Release(object attacker) => holders.Remove(attacker);
}
