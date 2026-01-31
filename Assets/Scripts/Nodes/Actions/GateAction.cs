using UnityEngine;
using UnityEngine.TextCore.Text;

public class GateAction : NodeAction
{
    [SerializeField] private Gate gate;

    public override void OnResolve(CharacterManager character, Cardinal direction, Node node, ActionEdge edge)
    {
        character.Movement.ApplyDirection(direction);

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
            character.Movement.Stop();
            return;
        }
    }
}