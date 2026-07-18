using System.Collections.Generic;
using UnityEngine;

public class ActionEdge : EdgeNode
{
    [SerializeField] ActionType actionType;

    public ActionType ActionType => actionType;

    private List<NodeAction> actions = new List<NodeAction>();

    public override EdgeTraversalResult GetTraversalResult(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        EdgeTraversalResult result = EdgeTraversalResult.Pass;

        foreach (var action in actions)
            result = Combine(result, action.GetTraversalResult(character, direction, node, this));

        return result;
    }

    public override EdgeTraversalResult Resolve(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        EdgeTraversalResult result = EdgeTraversalResult.Pass;

        foreach (var action in actions)
            result = Combine(result, action.Resolve(character, direction, node, this));

        return result;
    }

    public void Subscribe(NodeAction action)
    {
        if (!actions.Contains(action))
        {
            actions.Add(action);
        }
    }

    private static EdgeTraversalResult Combine(
        EdgeTraversalResult current,
        EdgeTraversalResult next)
    {
        if (current == EdgeTraversalResult.Blocked || next == EdgeTraversalResult.Blocked)
            return EdgeTraversalResult.Blocked;

        if (current == EdgeTraversalResult.Interact || next == EdgeTraversalResult.Interact)
            return EdgeTraversalResult.Interact;

        if (current == EdgeTraversalResult.Teleport || next == EdgeTraversalResult.Teleport)
            return EdgeTraversalResult.Teleport;

        return EdgeTraversalResult.Pass;
    }
}
