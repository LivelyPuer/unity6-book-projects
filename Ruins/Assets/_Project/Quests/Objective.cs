using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ruins/Objective")]
public class Objective : ScriptableObject
{
    public string title = "Пробудить алтари";
    public int required = 3;

    [NonSerialized] private readonly HashSet<string> done = new();
    public int Progress => done.Count;
    public bool Completed => done.Count >= required;

    public event Action<Objective> Changed;

    public void Report(string id)
    {
        if (Completed || !done.Add(id)) return;
        Changed?.Invoke(this);
    }

    public void ResetRuntime() => done.Clear();   // при новой игре
}
