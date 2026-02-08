using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(GhostContext))]
public abstract class GhostBehaviour : MonoBehaviour
{
    [SerializeField] protected float duration;

    public GhostContext Context {  get; private set; }

    private void Awake()
    {
        Context = GetComponent<GhostContext> ();
        enabled = false;
    }

    protected virtual void OnEnable()
    {
        if (Context?.Ghost?.Movement != null)
            Context.Ghost.Movement.DirectionResolver = ChooseDirectionAtNode;
    }

    protected virtual void OnDisable()
    {
        if (Context?.Ghost?.Movement != null &&
            Context.Ghost.Movement.DirectionResolver == ChooseDirectionAtNode)
            Context.Ghost.Movement.DirectionResolver = null;
    }

    public virtual void Enable()
    {
       Enable(duration);
    }

    public virtual void Enable(float duration)
    {
        enabled = true;

        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        enabled = false;
        CancelInvoke();
    }

    protected virtual Cardinal? ChooseDirectionAtNode(Node node, Cardinal[] exclude = null)
    {
        if (node == null) return null;

        int count = node.Edges.Count;
        if (count == 0) return null;

        Cardinal opposite = CardinalUtil.Opposite(Context.Ghost.Movement.Direction);
        bool avoidReverse = count > 1;

        if (TryChooseFromCandidates(node, exclude, avoidReverse, opposite, out Cardinal chosen))
            return chosen;

        if (avoidReverse &&
            TryChooseFromCandidates(node, exclude, false, opposite, out chosen))
            return chosen;

        return null;
    }

    protected abstract bool TryPickDirection(Node node, IReadOnlyList<Cardinal> candidates, out Cardinal chosen);

    private bool TryChooseFromCandidates(
        Node node,
        Cardinal[] exclude,
        bool avoidReverse,
        Cardinal opposite,
        out Cardinal chosen)
    {
        List<Cardinal> candidates = new();
        foreach (Cardinal available in node.Edges.Keys)
        {
            if (avoidReverse && available == opposite)
                continue;

            if (exclude != null && System.Array.IndexOf(exclude, available) >= 0)
                continue;

            candidates.Add(available);
        }

        if (candidates.Count == 0)
        {
            chosen = default;
            return false;
        }

        return TryPickDirection(node, candidates, out chosen);
    }
}
