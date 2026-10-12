using UnityEngine;

public class Shrine : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private ParticleSystem restVfx;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private int saveSlot = 0;

    public string GetPrompt(GameObject actor) => "Отдохнуть";
    public bool CanInteract(GameObject actor) => true;

    public void Interact(GameObject actor)
    {
        actor.GetComponent<Health>().ResetFull();
        actor.GetComponent<PlayerRespawn>().RespawnPoint = respawnPoint;
        restVfx.Play();
        saveSystem.Save(saveSlot);
        Toast.Show("Прогресс сохранён");
    }
}
