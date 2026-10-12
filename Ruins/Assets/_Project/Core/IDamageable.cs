using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount, Vector3 hitPoint);
    bool IsAlive { get; }
}
