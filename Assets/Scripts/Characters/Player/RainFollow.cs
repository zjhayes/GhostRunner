using System.Collections.Generic;
using UnityEngine;

public class RainFollow : MonoBehaviour
{
    private const float BoundaryContactTolerance = 0.001f;

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

            if (TryGetPaddedBounds(b.WorldBounds, out Bounds bounds) && bounds.Contains(point))
                return true;
        }

        // A padded boundary leaves a gap at a shared face. Add the shared
        // portions back so touching boundaries behave as one continuous shape.
        for (int i = 0; i < boundaries.Count; i++)
        {
            if (!boundaries[i]) continue;

            for (int j = i + 1; j < boundaries.Count; j++)
            {
                if (!boundaries[j]) continue;

                for (int axis = 0; axis < 3; axis++)
                {
                    if (TryGetConnectorBounds(
                            boundaries[i].WorldBounds,
                            boundaries[j].WorldBounds,
                            axis,
                            out Bounds connector) && connector.Contains(point))
                    {
                        return true;
                    }
                }
            }
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

            if (TryGetPaddedBounds(b.WorldBounds, out Bounds bounds))
                ConsiderClosestPoint(bounds, point, ref closestSqr, ref closest);
        }

        for (int i = 0; i < boundaries.Count; i++)
        {
            if (!boundaries[i]) continue;

            for (int j = i + 1; j < boundaries.Count; j++)
            {
                if (!boundaries[j]) continue;

                for (int axis = 0; axis < 3; axis++)
                {
                    if (TryGetConnectorBounds(
                            boundaries[i].WorldBounds,
                            boundaries[j].WorldBounds,
                            axis,
                            out Bounds connector))
                    {
                        ConsiderClosestPoint(connector, point, ref closestSqr, ref closest);
                    }
                }
            }
        }

        return closest;
    }

    private bool TryGetPaddedBounds(Bounds source, out Bounds paddedBounds)
    {
        float inset = Mathf.Max(0f, padding);
        Vector3 size = source.size - Vector3.one * (inset * 2f);
        if (size.x < 0f || size.y < 0f || size.z < 0f)
        {
            paddedBounds = default;
            return false;
        }

        paddedBounds = new Bounds(source.center, size);
        return true;
    }

    private bool TryGetConnectorBounds(Bounds a, Bounds b, int connectionAxis, out Bounds connector)
    {
        connector = default;

        Vector3 aMin = a.min;
        Vector3 aMax = a.max;
        Vector3 bMin = b.min;
        Vector3 bMax = b.max;

        // A connector is only valid when the source boxes touch or overlap on
        // every axis. A tiny tolerance avoids seams caused by float rounding.
        for (int axis = 0; axis < 3; axis++)
        {
            if (aMax[axis] + BoundaryContactTolerance < bMin[axis] ||
                bMax[axis] + BoundaryContactTolerance < aMin[axis])
            {
                return false;
            }
        }

        if (a.center[connectionAxis] > b.center[connectionAxis])
        {
            (aMin, bMin) = (bMin, aMin);
            (aMax, bMax) = (bMax, aMax);
        }

        // Only the axis whose opposing faces meet forms the passage. This
        // prevents the overlapping dimensions from producing extra connector
        // planes that could weaken padding along an exposed edge.
        if (Mathf.Abs(bMin[connectionAxis] - aMax[connectionAxis]) > BoundaryContactTolerance)
            return false;

        float inset = Mathf.Max(0f, padding);
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        for (int axis = 0; axis < 3; axis++)
        {
            if (axis == connectionAxis)
            {
                // Bridge the two independently inset faces.
                float firstInnerFace = aMax[axis] - inset;
                float secondInnerFace = bMin[axis] + inset;
                min[axis] = Mathf.Min(firstInnerFace, secondInnerFace);
                max[axis] = Mathf.Max(firstInnerFace, secondInnerFace);
            }
            else
            {
                // The whole shared face is internal to the combined volume.
                // Do not apply either boundary's padding along this opening.
                min[axis] = Mathf.Max(aMin[axis], bMin[axis]);
                max[axis] = Mathf.Min(aMax[axis], bMax[axis]);
            }

            if (max[axis] < min[axis])
                return false;
        }

        connector.SetMinMax(min, max);
        return true;
    }

    private static void ConsiderClosestPoint(
        Bounds bounds,
        Vector3 point,
        ref float closestSqr,
        ref Vector3 closest)
    {
        Vector3 clamped = bounds.ClosestPoint(point);
        float sqr = (point - clamped).sqrMagnitude;
        if (sqr < closestSqr)
        {
            closestSqr = sqr;
            closest = clamped;
        }
    }
}

