using UnityEngine;

public interface IInteractable
{
    string GetPrompt(GameObject actor);     // «Открыть», «Поднять»
    bool CanInteract(GameObject actor);
    void Interact(GameObject actor);
}
