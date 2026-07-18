using System.Linq;
using UnityEngine;

public abstract class NodeAction : GameBehaviour
{
    [Header("Edge Detection")]
    [SerializeField] int range = 1;
    [SerializeField] Vector3 offset;
    [SerializeField] CardinalMask directions = CardinalMask.All;
    [SerializeField] ActionType[] allowedActions;

    private void Start()
    {
        SubscribeToEdges();
    }

    public EdgeTraversalResult GetTraversalResult(
        CharacterManager character,
        Cardinal direction,
        Node node,
        ActionEdge edge)
    {
        if (!IsActionAllowed(edge))
            return EdgeTraversalResult.Pass;

        return character switch
        {
            PlayerManager player => GetPlayerTraversalResult(player, direction, node, edge),
            Ghost ghost => GetGhostTraversalResult(ghost, direction, node, edge),
            _ => EdgeTraversalResult.Pass
        };
    }

    public EdgeTraversalResult Resolve(
        CharacterManager character,
        Cardinal direction,
        Node node,
        ActionEdge edge)
    {
        EdgeTraversalResult result = GetTraversalResult(character, direction, node, edge);

        if (!IsActionAllowed(edge))
            return result;

        if (character is PlayerManager player)
            OnResolvePlayer(player, direction, node, edge);
        else if (character is Ghost ghost)
            OnResolveGhost(ghost, direction, node, edge);

        return result;
    }

    protected abstract EdgeTraversalResult GetPlayerTraversalResult(
        PlayerManager player,
        Cardinal direction,
        Node node,
        ActionEdge edge);

    protected abstract EdgeTraversalResult GetGhostTraversalResult(
        Ghost ghost,
        Cardinal direction,
        Node node,
        ActionEdge edge);

    protected abstract void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge);

    protected abstract void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge);

    protected virtual void SubscribeToEdges()
    {
        var nodeManager = GameManager.NodeManager;

        // Subscribe to tile beneath object.
        if (nodeManager.TryGetEdgeAtPosition<ActionEdge>(
            transform.position,
            offset,
            out var currentEdge) &&
            IsActionAllowed(currentEdge))
        {
            currentEdge.Subscribe(this);
        }

        // Subscribe to adjacent edges.
        foreach (Cardinal dir in CardinalUtil.Cardinals)
        {
            if (!IsDirectionEnabled(dir))
                continue;

            if (nodeManager.TryFindActionEdge(
                transform.position,
                offset,
                dir,
                range,
                out var actionEdge) &&
                IsActionAllowed(actionEdge))
            {
                actionEdge.Subscribe(this);
            }
        }
    }

    private bool IsActionAllowed(ActionEdge edge)
    {
        if (allowedActions == null || allowedActions.Length == 0)
            return true; // empty, accept all

        if (allowedActions.Contains(edge.ActionType))
            return true;

        return false;
    }

    private bool IsDirectionEnabled(Cardinal dir)
    {
        return dir switch
        {
            Cardinal.North => directions.HasFlag(CardinalMask.North),
            Cardinal.South => directions.HasFlag(CardinalMask.South),
            Cardinal.East => directions.HasFlag(CardinalMask.East),
            Cardinal.West => directions.HasFlag(CardinalMask.West),
            _ => false
        };
    }

}
