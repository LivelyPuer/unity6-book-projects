using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Полоса стража подсказывает окно уязвимости: золотая рамка и надпись
public class BossBarView : MonoBehaviour
{
    [SerializeField] private BossController boss;
    [SerializeField] private Image frame;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normal = new(0.06f, 0.05f, 0.08f, 0.72f);
    [SerializeField] private Color open = new(1f, 0.78f, 0.3f, 1f);

    void OnEnable()
    {
        boss.VulnerabilityChanged += Show;
        Show(boss.IsVulnerable);
    }

    void OnDisable() => boss.VulnerabilityChanged -= Show;

    private void Show(bool vulnerable)
    {
        frame.color = vulnerable ? open : normal;
        label.enabled = vulnerable;
    }

    void Update()
    {
        if (label.enabled)
            label.alpha = 0.65f + 0.35f * Mathf.Sin(Time.unscaledTime * 10f);
    }
}
