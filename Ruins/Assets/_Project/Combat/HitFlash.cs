using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private static readonly int EmissionId =
        Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color flashColor = Color.white * 3f;
    [SerializeField] private float duration = 0.08f;
    private MaterialPropertyBlock block;

    void Awake() => block = new MaterialPropertyBlock();

    public void Flash() => StartCoroutine(FlashRoutine());

    private IEnumerator FlashRoutine()
    {
        SetEmission(flashColor);
        // не зависим от hitstop
        yield return new WaitForSecondsRealtime(duration);
        // Снимаем блок свойств: с ним объект выпадает из SRP Batcher
        foreach (var r in renderers) r.SetPropertyBlock(null);
    }

    private void SetEmission(Color c)
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetColor(EmissionId, c);
            r.SetPropertyBlock(block);
        }
    }
}
