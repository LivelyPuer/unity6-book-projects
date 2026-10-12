using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private float radius = 1.6f;
    [SerializeField] private LayerMask interactableMask;

    private readonly Collider[] hits = new Collider[8];
    public IInteractable Current { get; private set; }
    public event Action<IInteractable> CurrentChanged;

    void OnEnable() => input.InteractPressed += TryInteract;
    void OnDisable() => input.InteractPressed -= TryInteract;

    void Update()
    {
        IInteractable best = null;
        float bestScore = float.MinValue;
        int n = Physics.OverlapSphereNonAlloc(transform.position, radius,
            hits, interactableMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < n; i++)
        {
            if (!hits[i].TryGetComponent(out IInteractable it)) continue;
            if (!it.CanInteract(gameObject)) continue;

            Vector3 to = hits[i].transform.position - transform.position;
            to.y = 0f;
            // Предпочитаем то, что впереди и ближе
            float score = Vector3.Dot(transform.forward, to.normalized)
                        - to.magnitude * 0.3f;
            if (score > bestScore) { bestScore = score; best = it; }
        }

        if (best != Current)
        {
            Current = best;
            CurrentChanged?.Invoke(Current);
        }
    }

    private void TryInteract()
    {
        if (Current != null && Current.CanInteract(gameObject))
            Current.Interact(gameObject);
    }
}
