using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private static readonly int AttackId = Animator.StringToHash("Attack");
    private static readonly int ComboId = Animator.StringToHash("ComboIndex");

    [SerializeField] private PlayerInputReader input;
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Animator animator;

    [Header("Хитбокс")]
    [SerializeField] private Transform hitPoint;     // у клинка меча
    [SerializeField] private float hitRadius = 0.9f;
    [SerializeField] private LayerMask hittable;

    [Header("Комбо")]
    [SerializeField] private float[] comboDamage = { 10f, 12f, 22f };
    [SerializeField] private float bufferTime = 0.3f;

    [Header("Отклик")]
    [SerializeField] private float lunge = 6f;         // рывок вперёд, м/с
    [SerializeField] private float aimRadius = 3.5f;   // доводка на цель
    [SerializeField] private TrailRenderer trail;      // шлейф клинка
    [SerializeField] private SoundData swingSound;

    private readonly Collider[] hits = new Collider[16];
    private readonly HashSet<IDamageable> hitThisSwing = new();
    private int comboIndex;
    private bool attacking, comboWindow, hitboxActive;
    private float bufferTimer;

    public event System.Action<IDamageable, Vector3> HitLanded;
    public bool IsFinisher => comboIndex == comboDamage.Length - 1;

    void OnEnable() => input.AttackPressed += BufferAttack;
    void OnDisable() => input.AttackPressed -= BufferAttack;

    private void BufferAttack() => bufferTimer = bufferTime;

    void Update()
    {
        if (bufferTimer > 0f)
        {
            bufferTimer -= Time.deltaTime;
            if (!attacking && motor.CanMove && motor.IsGrounded
                && !motor.IsDodging)
                StartAttack(0);
            else if (attacking && comboWindow
                     && comboIndex < comboDamage.Length - 1)
                StartAttack(comboIndex + 1);
        }
        if (hitboxActive) ApplyHits();
    }

    private void StartAttack(int index)
    {
        bufferTimer = 0f;
        comboIndex = index;
        attacking = true;
        comboWindow = false;
        motor.CanMove = false;
        AimAtNearest();
        motor.Nudge(transform.forward * lunge);
        animator.SetInteger(ComboId, index);
        animator.SetTrigger(AttackId);
    }

    // Развернуться к ближайшему врагу рядом: удар не уходит «в молоко»
    private void AimAtNearest()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position,
            aimRadius, hits, hittable, QueryTriggerInteraction.Collide);
        float best = float.MaxValue;
        Vector3 dir = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            Vector3 to = hits[i].transform.position - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude < best) { best = to.sqrMagnitude; dir = to; }
        }
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private void ApplyHits()
    {
        int count = Physics.OverlapSphereNonAlloc(hitPoint.position,
            hitRadius, hits, hittable, QueryTriggerInteraction.Collide);
        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent(out IDamageable target)) continue;
            if (!hitThisSwing.Add(target)) continue;   // один раз за взмах
            Vector3 point = hits[i].ClosestPoint(hitPoint.position);
            target.TakeDamage(comboDamage[comboIndex], point);
            HitLanded?.Invoke(target, point);
        }
    }

    // --- события анимации ---
    public void AttackStart()
    {
        hitThisSwing.Clear();
        hitboxActive = true;
        trail.emitting = true;
        SoundPlayer.Instance.Play(swingSound, hitPoint.position);
    }

    public void AttackEnd()
    {
        hitboxActive = false;
        trail.emitting = false;
    }

    public void ComboWindow() => comboWindow = true;

    public void AttackFinished(int index)
    {
        if (index != comboIndex) return;   // событие предыдущего клипа
        ResetCombo();
        motor.CanMove = true;
    }

    // Прервать удар извне (смерть, оглушение): клип не доиграет
    // до конца, и событие AttackFinished не придёт
    public void ResetCombo()
    {
        attacking = comboWindow = hitboxActive = false;
        comboIndex = 0;
        bufferTimer = 0f;
        trail.emitting = false;
    }

    void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = hitboxActive ? Color.red : new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }
}
