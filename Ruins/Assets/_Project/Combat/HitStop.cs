using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }
    private Coroutine running;
    private float resumeScale = 1f;

    void Awake() => Instance = this;

    public void Freeze(float duration, float scale = 0.02f)
    {
        if (Time.timeScale == 0f) return;          // игра на паузе
        if (running != null) StopCoroutine(running);
        else resumeScale = Time.timeScale;
        running = StartCoroutine(FreezeRoutine(duration, scale));
    }

    private IEnumerator FreezeRoutine(float duration, float scale)
    {
        Time.timeScale = scale;
        yield return new WaitForSecondsRealtime(duration);
        // Паузу, поставленную во время остановки кадра, не снимаем
        if (Time.timeScale == scale) Time.timeScale = resumeScale;
        running = null;
    }
}
