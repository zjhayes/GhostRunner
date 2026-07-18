using UnityEngine;

public class GateAction : NodeAction
{
    [SerializeField] private Gate gate;

    protected override EdgeTraversalResult GetPlayerTraversalResult(
        PlayerManager player,
        Cardinal direction,
        Node node,
        ActionEdge edge)
    {
        if (gate.IsPassable || edge.ActionType == ActionType.GATE_OPEN)
            return EdgeTraversalResult.Pass;

        return EdgeTraversalResult.Blocked;
    }

    protected override EdgeTraversalResult GetGhostTraversalResult(
        Ghost ghost,
        Cardinal direction,
        Node node,
        ActionEdge edge)
    {
        return EdgeTraversalResult.Blocked;
    }

    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
        if (gate.IsPassable)
        {
            return;
        }
        else if (edge.ActionType == ActionType.GATE_OPEN)
        {
            gate.Open();
        }
        else if (edge.ActionType == ActionType.BLOCKED)
        {
            gate.PlayLockedSound();
        }
    }

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
    }
}
