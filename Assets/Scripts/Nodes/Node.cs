using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Node : Cell
{
    public Dictionary<Cardinal, EdgeNode> Edges { get; private set; }

    private void Start()
    {
        Edges = NodeUtil.BuildEdgeMap(this, GameManager.NodeManager);
    }

    public EdgeTraversalResult ResolveEdge(CharacterManager character, Cardinal direction)
    {
        if (Edges.TryGetValue(direction, out var edge))
            return edge.Resolve(character, direction, this);

        return EdgeTraversalResult.Blocked;
    }

    public EdgeTraversalResult GetTraversalResult(CharacterManager character, Cardinal direction)
    {
        if (Edges.TryGetValue(direction, out var edge))
            return edge.GetTraversalResult(character, direction, this);

        return EdgeTraversalResult.Blocked;
    }
}
