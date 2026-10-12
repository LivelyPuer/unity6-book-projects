using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private string message = "Шипы оказались острыми!";

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            game.PlayerDied(message);
    }
}