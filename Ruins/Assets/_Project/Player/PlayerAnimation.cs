using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private static readonly int SpeedId = Animator.StringToHash("Speed");
    private static readonly int GroundedId = Animator.StringToHash("Grounded");
    private static readonly int JumpId = Animator.StringToHash("Jump");
    private static readonly int DodgeId = Animator.StringToHash("Dodge");
    private static readonly int HitId = Animator.StringToHash("Hit");
    private static readonly int DeadId = Animator.StringToHash("Dead");

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Health health;

    void OnEnable()
    {
        motor.Jumped += OnJumped;
        motor.Dodged += OnDodged;
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    void OnDisable()
    {
        motor.Jumped -= OnJumped;
        motor.Dodged -= OnDodged;
        health.Damaged -= OnDamaged;
        health.Died -= OnDied;
    }

    void Update()
    {
        animator.SetFloat(SpeedId, motor.PlanarSpeed, 0.1f, Time.deltaTime);
        animator.SetBool(GroundedId, motor.IsGrounded);
    }

    private void OnJumped() => animator.SetTrigger(JumpId);
    private void OnDodged() => animator.SetTrigger(DodgeId);
    private void OnDamaged(float amount, Vector3 point) =>
        animator.SetTrigger(HitId);
    private void OnDied() => animator.SetBool(DeadId, true);
}
