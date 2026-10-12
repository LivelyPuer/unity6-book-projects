using UnityEngine;
using UnityEngine.AI;

// Арена босса: при входе героя будит стража и показывает его полосу
public class BossArena : MonoBehaviour
{
    [SerializeField] private BossController boss;
    [SerializeField] private CanvasGroup bossBar;
    [SerializeField] private Health bossHealth;
    [SerializeField] private Health playerHealth;
    [SerializeField] private string bossName = "Страж руин";
    private bool started;
    private Vector3 home;
    private Quaternion homeRotation;

    void Start()
    {
        boss.enabled = false;
        bossBar.alpha = 0f;
        home = boss.transform.position;
        homeRotation = boss.transform.rotation;
        bossHealth.Died += OnBossDied;
        playerHealth.Died += OnPlayerDied;
    }

    void OnTriggerEnter(Collider other)
    {
        if (started || !other.CompareTag("Player")) return;
        started = true;
        boss.enabled = true;
        bossBar.alpha = 1f;
        CombatMusic.Engaged.Add(this);
        Toast.Show(bossName);
    }

    // Герой погиб: страж возвращается на место и восстанавливает силы
    private void OnPlayerDied()
    {
        if (!started || !bossHealth.IsAlive) return;
        started = false;
        boss.enabled = false;
        bossBar.alpha = 0f;
        CombatMusic.Engaged.Remove(this);
        boss.GetComponent<NavMeshAgent>().Warp(home);
        boss.transform.rotation = homeRotation;
        boss.ResetFight();
    }

    private void OnBossDied()
    {
        bossBar.alpha = 0f;
        CombatMusic.Engaged.Remove(this);
        Toast.Show("Страж повержен. Руины свободны!");
    }
}
