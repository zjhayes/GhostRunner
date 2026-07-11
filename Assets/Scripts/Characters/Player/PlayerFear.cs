using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerFear : MonoBehaviour
{
    public const float MaxFear = 100f;

    [SerializeField, Min(0.01f)] private float fearRadius = 6f;

    public float Fear { get; private set; }

    public event Action OnFrightened;

    private readonly List<Collider2D> nearbyGhosts = new();
    private Collider2D playerCollider;
    private ContactFilter2D ghostFilter;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
        ghostFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask(Layer.GHOSTS),
            useTriggers = true
        };

        Fear = 0f;
    }

    private void FixedUpdate()
    {
        SetFear(CalculateProximityFear());
    }

    private float CalculateProximityFear()
    {
        nearbyGhosts.Clear();
        Physics2D.OverlapCircle(transform.position, fearRadius, ghostFilter, nearbyGhosts);

        float closestDistance = fearRadius;

        foreach (Collider2D ghostCollider in nearbyGhosts)
        {
            if (ghostCollider == null)
                continue;

            ColliderDistance2D separation = playerCollider.Distance(ghostCollider);
            if (separation.isOverlapped)
                return MaxFear;

            closestDistance = Mathf.Min(closestDistance, separation.distance);
        }

        return MaxFear * (1f - Mathf.Clamp01(closestDistance / fearRadius));
    }

    public void AddFear(float amount)
    {
        SetFear(Fear + amount);
    }

    public void SetFear(float amount)
    {
        float previousFear = Fear;
        Fear = Mathf.Clamp(amount, 0f, MaxFear);

        if (previousFear < MaxFear && Fear >= MaxFear)
            OnFrightened?.Invoke();
    }

    public void ResetFear()
    {
        SetFear(0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fearRadius);
    }
}
