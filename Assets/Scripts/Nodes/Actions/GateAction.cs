using UnityEngine;

public class GateAction : NodeAction
{
    [SerializeField] private Gate gate;

    public override void OnResolve(MovementManager movement, Cardinal direction, Node node, ActionEdge edge)
    {
        movement.ApplyDirection(direction);
        
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
            movement.Stop();
            return;
        }
    }
}