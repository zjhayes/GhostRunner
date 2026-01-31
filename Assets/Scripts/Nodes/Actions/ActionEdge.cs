using System.Collections.Generic;
using UnityEngine;

public class ActionEdge : EdgeNode
{
    [SerializeField] ActionType actionType;

    public ActionType ActionType => actionType;

    private List<NodeAction> actions = new List<NodeAction>();

    public override void Resolve(CharacterManager character, Cardinal direction, Node node)
    {
        foreach (var action in actions)
        {
            action.Resolve(character, direction, node, this);
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
