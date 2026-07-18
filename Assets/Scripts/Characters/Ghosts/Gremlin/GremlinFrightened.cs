using System.Collections.Generic;
using UnityEngine;

public class GremlinFrightened : GhostBehaviour
{
    private GremlinContext GremlinContext => Context as GremlinContext;

    public void FleeFrom(Transform threat)
    {
        if (threat == null || Context?.Ghost?.Movement == null)
            return;

        Vector2 away = (Vector2)Context.Ghost.transform.position - (Vector2)threat.position;
        if (away == Vector2.zero)
            return;

        Context.Ghost.Movement.SetDirection(away);
    }

    protected override Cardinal? ChooseDirectionAtNode(Node node, Cardinal[] exclude = null)
    {
        if (node == null)
            return null;

        Cardinal current = Context.Ghost.Movement.Direction;

        if (!IsTraversable(node, current))
        {
            GremlinContext?.Hide();
            return null;
        }

        List<Cardinal> candidates = new();
        foreach (Cardinal available in node.Edges.Keys)
        {
            if (exclude != null && System.Array.IndexOf(exclude, available) >= 0)
                continue;

            if (!IsTraversable(node, available))
                continue;

            candidates.Add(available);
        }

        if (candidates.Count == 0)
        {
            GremlinContext?.Hide();
            return null;
        }

        return TryPickDirection(node, candidates, out Cardinal chosen)
            ? chosen
            : null;
    }

    private bool IsTraversable(Node node, Cardinal direction)
    {
        if (!node.Edges.TryGetValue(direction, out EdgeNode edge))
            return false;

        EdgeTraversalResult result = edge.GetTraversalResult(Context.Ghost, direction, node);
        return result == EdgeTraversalResult.Pass || result == EdgeTraversalResult.Teleport;
    }

    protected override bool TryPickDirection(
        Node node,
        IReadOnlyList<Cardinal> candidates,
        out Cardinal chosen)
    {
        chosen = default;

        if (candidates.Count == 0 || Context.Ghost.Target == null)
            return false;

        Vector3 playerPosition = Context.Ghost.Target.position;

        bool hasBest = false;
        float bestDistance = float.MinValue;

        foreach (Cardinal candidate in candidates)
        {
            Vector2 direction = CardinalUtil.ToVector(candidate);

            Vector3 testPosition =
                node.transform.position + new Vector3(direction.x, direction.y, 0f);

            float distanceFromPlayer =
                (testPosition - playerPosition).sqrMagnitude;

            if (distanceFromPlayer > bestDistance)
            {
                bestDistance = distanceFromPlayer;
                chosen = candidate;
                hasBest = true;
            }
        }

        return hasBest;
    }

    public override void Enable()
    {
        enabled = true;
        CancelInvoke();
    }

    public override void Enable(float duration)
    {
        enabled = true;
        CancelInvoke();
    }
}
