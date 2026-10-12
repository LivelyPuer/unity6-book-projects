using UnityEngine;

// Подбираемый предмет покачивается и вращается — так его видно издалека
public class PickupBob : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.15f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float spin = 90f;
    private Vector3 start;

    void Start() => start = transform.localPosition;

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency * 2f * Mathf.PI) * amplitude;
        transform.localPosition = start + Vector3.up * y;
        transform.Rotate(0f, spin * Time.deltaTime, 0f, Space.World);
    }
}
