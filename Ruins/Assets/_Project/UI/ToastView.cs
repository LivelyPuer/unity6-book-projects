using TMPro;
using UnityEngine;

public class ToastView : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text label;
    [SerializeField] private float duration = 2.5f;
    private float timer;

    void OnEnable() => Toast.Shown += Show;
    void OnDisable() => Toast.Shown -= Show;
    void Start() => group.alpha = 0f;

    private void Show(string message)
    {
        label.text = message;
        timer = duration;
    }

    void Update()
    {
        timer -= Time.unscaledDeltaTime;
        group.alpha = Mathf.Clamp01(timer / 0.4f);   // плавное исчезание
    }
}
