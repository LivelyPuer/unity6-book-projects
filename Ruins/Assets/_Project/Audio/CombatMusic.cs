using System.Collections.Generic;
using UnityEngine;

public class CombatMusic : MonoBehaviour
{
    public static readonly HashSet<object> Engaged = new();

    [SerializeField] private AudioSource explore, combat;   // синхронный старт
    [SerializeField] private float fadeSpeed = 0.7f;

    void Start()
    {
        double start = AudioSettings.dspTime + 0.1;
        explore.PlayScheduled(start);
        combat.PlayScheduled(start);
        combat.volume = 0f;
    }

    void Update()
    {
        float target = Engaged.Count > 0 ? 1f : 0f;
        combat.volume = Mathf.MoveTowards(combat.volume, target,
                                          fadeSpeed * Time.deltaTime);
        explore.volume = 1f - combat.volume * 0.6f;
    }

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => Engaged.Clear();
}
