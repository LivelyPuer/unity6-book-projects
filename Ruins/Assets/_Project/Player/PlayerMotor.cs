using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private Health health;
    [SerializeField] private Transform cameraTransform;

    [Header("Бег")]
    [SerializeField] private float moveSpeed = 5.5f;     // обычный бег
    [SerializeField] private float sprintSpeed = 8.5f;   // с Shift
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSharpness = 16f;

    [Header("Прыжок")]
    [SerializeField] private float jumpHeight = 1.1f;
    [SerializeField] private float gravity = -25f;

    [Header("Перекат")]
    [SerializeField] private float dodgeSpeed = 11f;
    [SerializeField] private float dodgeDuration = 0.45f;
    [SerializeField] private float invulnerableTime = 0.3f;
    [SerializeField] private float dodgeCooldown = 0.2f;

    private CharacterController controller;
    private Vector3 planarVelocity;
    private float verticalVelocity;
    private Vector3 dodgeDirection;
    private float dodgeTimer, cooldownTimer;

    public bool CanMove { get; set; } = true;
    public bool IsGrounded => controller.isGrounded;
    public bool IsDodging => dodgeTimer > 0f;
    public float PlanarSpeed => planarVelocity.magnitude;
    public float VerticalSpeed => verticalVelocity;

    public event Action Jumped;
    public event Action Dodged;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void OnEnable()
    {
        input.JumpPressed += TryJump;
        input.DodgePressed += TryDodge;
    }

    void OnDisable()
    {
        input.JumpPressed -= TryJump;
        input.DodgePressed -= TryDodge;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        cooldownTimer -= dt;

        if (IsDodging) UpdateDodge(dt);
        else UpdateWalk(dt);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * dt;

        controller.Move((planarVelocity + Vector3.up * verticalVelocity) * dt);
    }

    private void UpdateWalk(float dt)
    {
        Vector3 wish = Vector3.zero;
        if (CanMove)
        {
            wish = CameraRelative(input.Move)
                 * (input.Sprint ? sprintSpeed : moveSpeed);
            if (wish.sqrMagnitude > 0.01f)
                FaceDirection(wish, dt);
        }
        planarVelocity = Vector3.MoveTowards(planarVelocity, wish,
                                             acceleration * dt);
    }

    private void UpdateDodge(float dt)
    {
        dodgeTimer -= dt;
        planarVelocity = dodgeDirection * dodgeSpeed;
        bool invulnerable = dodgeTimer > dodgeDuration - invulnerableTime;
        health.Invulnerable = invulnerable && dodgeTimer > 0f;
        if (dodgeTimer <= 0f)
        {
            health.Invulnerable = false;
            planarVelocity = dodgeDirection * moveSpeed;
        }
    }

    private void TryJump()
    {
        if (!CanMove || !controller.isGrounded || IsDodging) return;
        verticalVelocity = Mathf.Sqrt(2f * jumpHeight * -gravity);
        Jumped?.Invoke();
    }

    private void TryDodge()
    {
        if (!CanMove || !controller.isGrounded || IsDodging) return;
        if (cooldownTimer > 0f) return;

        Vector3 dir = CameraRelative(input.Move);
        dodgeDirection = dir.sqrMagnitude > 0.01f ? dir.normalized
                                                  : transform.forward;
        transform.rotation = Quaternion.LookRotation(dodgeDirection);
        dodgeTimer = dodgeDuration;
        cooldownTimer = dodgeDuration + dodgeCooldown;
        Dodged?.Invoke();
    }

    private Vector3 CameraRelative(Vector2 move)
    {
        Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
        Vector3 r = cameraTransform.right;   r.y = 0f; r.Normalize();
        return f * move.y + r * move.x;
    }

    private void FaceDirection(Vector3 direction, float dt)
    {
        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, target,
            1f - Mathf.Exp(-turnSharpness * dt));
    }

    // Толчок, который затухает с ускорением, — рывок при ударе
    public void Nudge(Vector3 velocity) => planarVelocity = velocity;

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;   // иначе контроллер «откатит» позицию
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;
        planarVelocity = Vector3.zero;
        verticalVelocity = 0f;
    }
}
