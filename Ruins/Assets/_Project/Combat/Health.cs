using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float max = 100f;
    [SerializeField] private float hitInvulnerability = 0.4f;

    public float Current { get; private set; }
    public float Max => max;
    public bool IsAlive => Current > 0f;
    public bool Invulnerable { get; set; }
    public float DamageScale { get; set; } = 1f;   // броня, окна уязвимости

    public event Action<float, float> Changed;
    public event Action<float, Vector3> Damaged;   // урон, точка удара
    public event Action Died;

    private float invulnerableUntil;

    void Awake() => Current = max;

    public void TakeDamage(float amount, Vector3 hitPoint)
    {
        if (!IsAlive || Invulnerable || Time.time < invulnerableUntil) return;

        amount *= DamageScale;
        Current = Mathf.Max(0f, Current - amount);
        invulnerableUntil = Time.time + hitInvulnerability;
        Damaged?.Invoke(amount, hitPoint);
        Changed?.Invoke(Current, max);
        if (!IsAlive) Died?.Invoke();
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;
        Current = Mathf.Min(max, Current + amount);
        Changed?.Invoke(Current, max);
    }

    public void ResetFull()
    {
        Current = max;
        Changed?.Invoke(Current, max);
    }
}
