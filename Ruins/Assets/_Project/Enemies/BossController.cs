using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossAttack
{
    public string trigger;          // триггер Animator
    public float minRange, maxRange;
    public float windup = 0.6f;     // подготовка (информация для дизайна)
    public float recovery = 1f;     // окно уязвимости
    public int phase = 1;           // доступна с этой фазы
}

public class BossController : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] private List<BossAttack> attacks;
    [SerializeField] private float phase2Threshold = 0.5f;
    [SerializeField] private float phase2SpeedUp = 1.25f;

    public int Phase { get; private set; } = 1;
    public event System.Action<int> PhaseChanged;

    private float busyTimer;
    private BossAttack last;

    void Start()
    {
        if (!agent.isOnNavMesh) agent.Warp(transform.position);
    }

    void OnEnable() => health.Changed += OnHealthChanged;
    void OnDisable() => health.Changed -= OnHealthChanged;

    void Update()
    {
        if (!health.IsAlive) return;
        busyTimer -= Time.deltaTime;
        if (busyTimer > 0f) return;

        float distance = Vector3.Distance(transform.position, player.position);
        BossAttack attack = PickAttack(distance);
        if (attack == null)
        {
            agent.SetDestination(player.position);   // сблизиться
            return;
        }

        agent.ResetPath();
        FacePlayer();
        animator.SetTrigger(attack.trigger);
        float speed = Phase == 2 ? phase2SpeedUp : 1f;
        busyTimer = (attack.windup + attack.recovery + 0.5f) / speed;
        last = attack;
    }

    private BossAttack PickAttack(float distance)
    {
        var options = new List<BossAttack>();
        foreach (var a in attacks)
            if (a.phase <= Phase && distance >= a.minRange
                && distance <= a.maxRange && a != last)  // без повтора подряд
                options.Add(a);
        return options.Count > 0 ? options[Random.Range(0, options.Count)]
                                 : null;
    }

    private void OnHealthChanged(float current, float max)
    {
        if (Phase == 1 && current / max <= phase2Threshold)
        {
            Phase = 2;
            animator.SetTrigger("Roar");
            animator.speed = phase2SpeedUp;   // всё быстрее
            busyTimer = 2f;                    // пауза на смену фазы
            health.Invulnerable = true;
            Invoke(nameof(EndPhaseTransition), 2f);
            PhaseChanged?.Invoke(Phase);
        }
    }

    private void EndPhaseTransition() => health.Invulnerable = false;

    // Сброс боя (герой погиб): снова первая фаза и обычная скорость
    public void ResetFight()
    {
        CancelInvoke();
        Phase = 1;
        animator.speed = 1f;
        busyTimer = 0f;
        last = null;
        health.Invulnerable = false;
        health.ResetFull();
    }

    private void FacePlayer()
    {
        Vector3 to = player.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(to);
    }
}
