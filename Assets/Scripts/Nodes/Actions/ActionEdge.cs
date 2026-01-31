using System.Collections.Generic;
using UnityEngine;

public class ActionEdge : EdgeNode
{
    [SerializeField] ActionType actionType;

    public ActionType ActionType => actionType;

    private List<NodeAction> actions = new List<NodeAction>();

    public override void Resolve(MovementManager movement, Cardinal direction, Node node)
    {
        foreach (var action in actions)
        {
            action.Resolve(movement, direction, node, this);
        }
    }

    public void Subscribe(NodeAction action)
    {
        if (!actions.Contains(action))
        {
            actions.Add(action);
        }
    }
}
