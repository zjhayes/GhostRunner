using System;

[Obsolete("Use CheckpointEdge instead.")]
public class CheckpointAction : NodeAction
{
    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
    }

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
    }
}
