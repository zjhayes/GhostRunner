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

    public void ResolveEdge(CharacterManager character, Cardinal direction)
    {
        if (Edges.TryGetValue(direction, out var edge))
        {
            edge.Resolve(character, direction, this);
        }
    }
}
