using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool Sprint { get; private set; }

    public event Action JumpPressed;
    public event Action AttackPressed;
    public event Action DodgePressed;
    public event Action InteractPressed;

    private InputAction move, sprint, jump, attack, dodge, interact;

    void Awake()
    {
        var a = InputSystem.actions;
        move = a.FindAction("Player/Move");
        sprint = a.FindAction("Player/Sprint");
        jump = a.FindAction("Player/Jump");
        attack = a.FindAction("Player/Attack");
        dodge = a.FindAction("Player/Dodge");
        interact = a.FindAction("Player/Interact");
    }

    void Update()
    {
        Move = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
        Sprint = sprint.IsPressed();
        if (jump.WasPressedThisFrame()) JumpPressed?.Invoke();
        if (attack.WasPressedThisFrame()) AttackPressed?.Invoke();
        if (dodge.WasPressedThisFrame()) DodgePressed?.Invoke();
        if (interact.WasPressedThisFrame()) InteractPressed?.Invoke();
    }
}
