using UnityEngine;

public class SanctumGate : MonoBehaviour
{
    [SerializeField] private Objective objective;
    [SerializeField] private LockedDoor door;

    void OnEnable() => objective.Changed += OnChanged;
    void OnDisable() => objective.Changed -= OnChanged;

    private void OnChanged(Objective o)
    {
        if (o.Completed) door.SetOpen(true);
    }
}
