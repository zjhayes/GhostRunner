using UnityEngine;

public abstract class NodeAction : MonoBehaviour
{
    [Header("Edge Detection")]
    [SerializeField] int range = 3;
    [SerializeField] private Vector3 offset;
    [SerializeField] private CardinalMask directions = CardinalMask.All;

    private void Start()
    {
        SubscribeToEdges();
    }

    public void Resolve(MovementManager movement, Cardinal direction, Node node, ActionEdge edge)
    {
        OnResolve(movement, direction, node, edge);
    }
    public abstract void OnResolve(MovementManager movement, Cardinal direction, Node node, ActionEdge edge);

    protected virtual void SubscribeToEdges()
    {
        var nodeManager = GameManager.Instance.NodeManager;

        // Subscribe to tile beneath object.
        if (nodeManager.TryGetEdgeAtPosition<ActionEdge>(
            transform.position,
            offset,
            out var currentEdge))
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
                out var actionEdge
            ))
            {
                actionEdge.Subscribe(this);
            }
        }
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
