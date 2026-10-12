using UnityEngine;
using UnityEngine.AI;

public class EnemyHitReaction : MonoBehaviour
{
    private static readonly int HitId = Animator.StringToHash("Hit");
    private static readonly int DeadId = Animator.StringToHash("Dead");

    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Behaviour brain;  // EnemyBrain или BossController
    [SerializeField] private float knockback = 2.5f;
    [SerializeField] private GameObject[] dropOnDeath;

    [Header("Отклик")]
    [SerializeField] private HitFlash flash;
    [SerializeField] private SoundData hitSound, deathSound;
    [SerializeField] private GameObject deathVfx;

    void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Died -= OnDied;
    }

    private void OnDamaged(float amount, Vector3 point)
    {
        animator.SetTrigger(HitId);
        flash.Flash();
        SoundPlayer.Instance.Play(hitSound, point);
        Vector3 away = transform.position - point;
        away.y = 0f;
        agent.Move(away.normalized * knockback * 0.1f); // небольшой отброс
    }

    private void OnDied()
    {
        animator.SetBool(DeadId, true);
        SoundPlayer.Instance.Play(deathSound, transform.position);
        if (deathVfx != null)
            Destroy(Instantiate(deathVfx, transform.position,
                                Quaternion.identity), 3f);
        brain.enabled = false;
        agent.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;
        foreach (var drop in dropOnDeath)
            Instantiate(drop, transform.position + Vector3.up,
                        Quaternion.identity);
        Destroy(gameObject, 4f);
    }
}
