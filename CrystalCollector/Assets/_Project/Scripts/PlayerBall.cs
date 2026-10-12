using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlayerBall : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveForce = 14f;
    [SerializeField] private float maxSpeed = 8f;

    [Header("Прыжок")]
    [SerializeField] private float jumpSpeed = 6f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Ссылки")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private float radius;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private bool jumpQueued;

    public bool ControlEnabled { get; set; } = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius
                 * transform.localScale.x;
        moveAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!ControlEnabled)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.WasPressedThisFrame())
            jumpQueued = true;
    }

    void FixedUpdate()
    {
        Vector3 direction = CameraRelative(moveInput);

        // Разгоняемся, только пока не достигли максимальной скорости
        // (или если пытаемся тормозить/поворачивать)
        Vector3 flat = rb.linearVelocity;
        flat.y = 0f;
        bool belowLimit = flat.magnitude < maxSpeed;
        bool braking = Vector3.Dot(flat, direction) < 0f;
        if (belowLimit || braking)
            rb.AddForce(direction * moveForce, ForceMode.Acceleration);

        if (jumpQueued)
        {
            if (IsGrounded())
            {
                Vector3 v = rb.linearVelocity;
                v.y = jumpSpeed;
                rb.linearVelocity = v;
            }
            jumpQueued = false;
        }
    }

    private Vector3 CameraRelative(Vector2 input)
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 dir = right * input.x + forward * input.y;
        return Vector3.ClampMagnitude(dir, 1f);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down,
            radius + 0.1f, groundMask, QueryTriggerInteraction.Ignore);
    }
}