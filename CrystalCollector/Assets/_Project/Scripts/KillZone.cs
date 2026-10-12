using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private string message = "Вы упали!";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            game.PlayerDied(message);
    }
}