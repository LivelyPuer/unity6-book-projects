using UnityEngine;

public class Altar : MonoBehaviour, IInteractable, ISaveable
{
    [SerializeField] private Objective objective;
    [SerializeField] private SaveableId saveId;
    [SerializeField] private ParticleSystem awakenVfx;
    [SerializeField] private Light glow;
    private bool awakened;

    public string GetPrompt(GameObject actor) => "Пробудить алтарь";
    public bool CanInteract(GameObject actor) => !awakened;

    public void Interact(GameObject actor) => SetAwakened(true, true);

    private void SetAwakened(bool value, bool withEffects)
    {
        awakened = value;
        glow.enabled = value;
        if (!value) return;
        if (withEffects) awakenVfx.Play();
        objective.Report(saveId.Id);
    }

    public string CaptureJson() => awakened ? "1" : "0";
    public void RestoreJson(string json) => SetAwakened(json == "1", false);
}
