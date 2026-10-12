using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [SerializeField] private int poolSize = 24;
    private readonly List<AudioSource> pool = new();
    private int next;

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"Sfx_{i}");
            go.transform.SetParent(transform);
            pool.Add(go.AddComponent<AudioSource>());
        }
    }

    public void Play(SoundData sound, Vector3 position)
    {
        if (sound == null || sound.clips.Length == 0) return;
        if (Time.unscaledTime - sound.lastPlayTime < sound.cooldown) return;
        sound.lastPlayTime = Time.unscaledTime;

        AudioSource src = pool[next];
        next = (next + 1) % pool.Count; // по кругу: старые звуки вытесняются

        src.transform.position = position;
        src.clip = sound.PickClip();
        src.volume = sound.volume;
        src.pitch = Random.Range(sound.pitchRange.x, sound.pitchRange.y);
        src.spatialBlend = sound.spatialBlend;
        src.outputAudioMixerGroup = sound.output;
        src.Play();
    }
}
