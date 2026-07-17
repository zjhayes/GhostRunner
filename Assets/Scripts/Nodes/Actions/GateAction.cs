using UnityEngine;

public class GateAction : NodeAction
{
    [SerializeField] private Gate gate;

    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
        if (gate.IsOpen)
        {
            return;
        }
        else if (edge.ActionType == ActionType.GATE_OPEN)
        {
            gate.Open();
        }
        else if(edge.ActionType == ActionType.BLOCKED)
        {
            player.Movement.ApplyDirection(direction); // Face gate.
            player.Movement.Stop();
            return;
        }
    }

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
        ghost.TurnAround();
    }
}
