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

    public void Resolve(CharacterManager character, Cardinal direction, Node node, ActionEdge edge)
    {
        if (allowedActions.Contains(edge.ActionType))
        {
            OnResolve(character, direction, node, edge);
        }
    }

    public void OnResolve(CharacterManager character, Cardinal direction, Node node, ActionEdge edge)
    {
        if (character is PlayerManager player)
        {
            OnResolvePlayer(player, direction, node, edge);
        }
        else if (character is Ghost ghost)
        {
            OnResolveGhost(ghost, direction, node, edge);
        }
    }

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
