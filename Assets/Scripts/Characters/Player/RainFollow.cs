using System.Collections.Generic;
using UnityEngine;

public class RainFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Allowed Rain Boundaries")]
    [SerializeField] private List<RainBoundary> boundaries = new();

    [Header("Offsets")]
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 10f, 0f);
    [SerializeField] private float padding = 2f;

    void LateUpdate()
    {
        if (!target || boundaries.Count == 0)
            return;

        Vector3 desired = target.position + followOffset;

        // If target is inside any boundary, follow.
        if (IsInsideAnyBoundary(desired))
        {
            transform.position = desired;
            return;
        }

        // Otherwise clamp to nearest boundary surface
        Vector3 nearest = FindNearestBoundaryPoint(desired);
        transform.position = nearest;
    }

    private bool IsInsideAnyBoundary(Vector3 point)
    {
        foreach (var b in boundaries)
        {
            if (!b) continue;

            Bounds bounds = ExpandBounds(b.WorldBounds, -padding);
            if (bounds.Contains(point))
                return true;
        }
        return false;
    }

    private Vector3 FindNearestBoundaryPoint(Vector3 point)
    {
        float closestSqr = float.MaxValue;
        Vector3 closest = transform.position;

        foreach (var b in boundaries)
        {
            if (!b) continue;

            Bounds bounds = ExpandBounds(b.WorldBounds, -padding);
            Vector3 clamped = bounds.ClosestPoint(point);

            float sqr = (point - clamped).sqrMagnitude;
            if (sqr < closestSqr)
            {
                closestSqr = sqr;
                closest = clamped;
            }
        }

        return closest;
    }

    private Bounds ExpandBounds(Bounds b, float amount)
    {
        b.Expand(amount * 2f);
        return b;
    }
}

