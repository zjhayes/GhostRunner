using System.Collections.Generic;
using UnityEngine;

public class GhostScatter : GhostBehaviour
{
    protected override bool TryPickDirection(Node node, IReadOnlyList<Cardinal> candidates, out Cardinal chosen)
    {
        chosen = default;
        if (candidates.Count == 0)
            return false;

        int pickIndex = Random.Range(0, candidates.Count);
        chosen = candidates[pickIndex];
        return true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!Context.SuppressBehaviourTransitions)
            Context.Chase.Enable();
    }
}


