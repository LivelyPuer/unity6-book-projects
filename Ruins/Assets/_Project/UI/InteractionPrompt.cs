using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text label;

    void OnEnable() => interactor.CurrentChanged += Show;
    void OnDisable() => interactor.CurrentChanged -= Show;
    void Start() => Show(null);

    private void Show(IInteractable target)
    {
        group.alpha = target == null ? 0f : 1f;
        if (target != null)
        {
            string prompt = target.GetPrompt(interactor.gameObject);
            label.text = $"<b>[E]</b>  {prompt}";
        }
    }
}
