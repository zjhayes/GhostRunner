using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Node : Cell
{
    public Dictionary<Cardinal, EdgeNode> Edges { get; private set; }

    private void Start()
    {
        Edges = NodeUtil.BuildEdgeMap(this, GameManager.Instance.NodeManager);
    }

    public void ResolveEdge(MovementManager movement, Cardinal direction)
    {
        if (Edges.TryGetValue(direction, out var edge))
        {
            edge.Resolve(movement, direction, this);
        }
    }
}
