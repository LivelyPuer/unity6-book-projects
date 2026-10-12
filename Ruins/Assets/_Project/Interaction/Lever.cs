using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable, ISaveable
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onPulled;
    private bool pulled;

    public string GetPrompt(GameObject actor) => "Потянуть";
    public bool CanInteract(GameObject actor) => !pulled;

    public void Interact(GameObject actor)
    {
        pulled = true;
        animator.SetBool("Pulled", true);
        onPulled.Invoke();
    }

    public string CaptureJson() => pulled ? "1" : "0";

    public void RestoreJson(string json)
    {
        pulled = json == "1";
        animator.SetBool("Pulled", pulled);
        if (pulled) onPulled.Invoke();   // восстановить последствия
    }
}
