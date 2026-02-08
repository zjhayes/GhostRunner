using System.Collections.Generic;
using UnityEngine;

public class GhostChase : GhostBehaviour
{
    protected override bool TryPickDirection(Node node, IReadOnlyList<Cardinal> candidates, out Cardinal chosen)
    {
        chosen = default;
        if (candidates.Count == 0)
            return false;

        bool hasBest = false;
        float minDistance = float.MaxValue;

        foreach (Cardinal available in candidates)
        {
            Vector2 step = CardinalUtil.ToVector(available);

            // Predict the next tile/step in that direction.
            Vector3 newPosition = node.transform.position + new Vector3(step.x, step.y, 0f);

            float distance = (Context.Ghost.Target.position - newPosition).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                chosen = available;
                hasBest = true;
            }
        }

        return hasBest;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Context.Scatter.Enable();
    }
}
