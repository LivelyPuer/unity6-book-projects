using System;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public static event Action<Crystal> Collected;

    [SerializeField] private float spinSpeed = 120f;
    [SerializeField] private float bobAmplitude = 0.2f;
    [SerializeField] private float bobFrequency = 0.8f;
    [SerializeField] private GameObject collectVfx;

    private Vector3 startPosition;
    private float phase;
    private bool collected;

    void Start()
    {
        startPosition = transform.position;
        phase = UnityEngine.Random.value * 10f; // кристаллы не синхронны
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        float t = (Time.time + phase) * bobFrequency * 2f * Mathf.PI;
        transform.position = startPosition
                           + Vector3.up * (Mathf.Sin(t) * bobAmplitude);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        if (collectVfx != null)
            Instantiate(collectVfx, transform.position, Quaternion.identity);

        Collected?.Invoke(this);
        Destroy(gameObject);
    }
}