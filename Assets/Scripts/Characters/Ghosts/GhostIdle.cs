using System.Collections.Generic;
using UnityEngine;

public class GhostIdle : GhostBehaviour
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Context?.Ghost?.Movement?.Stop();
    }

    public override void Enable()
    {
        enabled = true;
        CancelInvoke();
    }

    public override void Enable(float duration)
    {
        Enable();
    }

    protected override bool TryPickDirection(
        Node node,
        IReadOnlyList<Cardinal> candidates,
        out Cardinal chosen)
    {
        chosen = default;
        return false;
    }
}
