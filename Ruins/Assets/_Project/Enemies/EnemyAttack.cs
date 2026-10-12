using System.Collections.Generic;
using UnityEngine;

// Хитбокс врага: как у героя, включается событиями анимации удара
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitRadius = 0.8f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private LayerMask hittable;
    [SerializeField] private TrailRenderer trail;      // красный шлейф удара
    [SerializeField] private SoundData swingSound;

    private readonly Collider[] hits = new Collider[8];
    private readonly HashSet<IDamageable> hitThisSwing = new();
    private bool active;
    private float activeUntil;

    // --- события анимации ---
    public void AttackStart()
    {
        hitThisSwing.Clear();
        SetActive(true);
        activeUntil = Time.time + 0.6f;   // если удар прервали
        SoundPlayer.Instance.Play(swingSound, hitPoint.position);
    }

    public void AttackEnd() => SetActive(false);

    private void SetActive(bool value)
    {
        active = value;
        trail.emitting = value;
    }

    void Update()
    {
        if (!active) return;
        if (Time.time > activeUntil) { SetActive(false); return; }
        int n = Physics.OverlapSphereNonAlloc(hitPoint.position, hitRadius,
            hits, hittable, QueryTriggerInteraction.Collide);
        for (int i = 0; i < n; i++)
        {
            if (!hits[i].TryGetComponent(out IDamageable target)) continue;
            if (!hitThisSwing.Add(target)) continue;
            Vector3 point = hits[i].ClosestPoint(hitPoint.position);
            target.TakeDamage(damage, point);
        }
    }

    void OnDisable() => SetActive(false);

    void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = active ? Color.red : new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }
}
