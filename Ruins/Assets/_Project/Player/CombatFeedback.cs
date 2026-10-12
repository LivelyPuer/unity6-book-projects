using Unity.Cinemachine;
using UnityEngine;

// Отклик боя героя: искры, звук, остановка кадра и тряска камеры
public class CombatFeedback : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private Health health;
    [SerializeField] private CinemachineImpulseSource impulse;
    [SerializeField] private HitFlash hurtFlash;   // красная вспышка героя

    [Header("Попадание: обычное, сильное, по броне")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject heavyVfx;
    [SerializeField] private GameObject armorVfx;
    [SerializeField] private SoundData heavySound, armorSound;

    [Header("Урон по герою")]
    [SerializeField] private SoundData hurtSound;

    void OnEnable()
    {
        combat.HitLanded += OnHitLanded;
        health.Damaged += OnHurt;
    }

    void OnDisable()
    {
        combat.HitLanded -= OnHitLanded;
        health.Damaged -= OnHurt;
    }

    private void OnHitLanded(IDamageable target, Vector3 point)
    {
        float scale = target is Health h ? h.DamageScale : 1f;
        if (scale < 1f)                          // броня: звон, без паузы
        {
            Spawn(armorVfx, point);
            SoundPlayer.Instance.Play(armorSound, point);
            impulse.GenerateImpulse(0.15f);
        }
        else if (scale > 1f || combat.IsFinisher) // сильный удар
        {
            Spawn(heavyVfx, point);
            SoundPlayer.Instance.Play(heavySound, point);
            HitStop.Instance.Freeze(0.1f);
            impulse.GenerateImpulse(0.6f);
        }
        else
        {
            Spawn(hitVfx, point);                // звук даёт сама цель
            HitStop.Instance.Freeze(0.05f);
            impulse.GenerateImpulse(0.25f);
        }
    }

    private void OnHurt(float amount, Vector3 point)
    {
        hurtFlash.Flash();
        SoundPlayer.Instance.Play(hurtSound, transform.position);
        HitStop.Instance.Freeze(0.06f);
        impulse.GenerateImpulse(0.8f);
    }

    private static void Spawn(GameObject prefab, Vector3 point)
    {
        if (prefab != null)
            Destroy(Instantiate(prefab, point, Quaternion.identity), 2f);
    }
}
