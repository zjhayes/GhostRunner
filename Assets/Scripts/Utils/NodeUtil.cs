using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class NodeUtil
{
    public static Dictionary<Vector3Int, Node> GetAllNodes(Tilemap tilemap)
    {
        var result = new Dictionary<Vector3Int, Node>();
        if (tilemap == null) return result;

        var bounds = tilemap.cellBounds;

        foreach (var cell in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(cell))
                continue;

            var go = tilemap.GetInstantiatedObject(cell);
            if (go == null)
                continue;

            if (go.TryGetComponent<Node>(out var node))
                result[cell] = node;
        }

        return result;
    }

    public static void GetAllNodes(
        Tilemap tilemap,
        out Dictionary<Vector3Int, Node> nodes,
        out Dictionary<Vector3Int, EdgeNode> edges)
    {
        nodes = new();
        edges = new();

        if (tilemap == null) return;

        foreach (var cell in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(cell))
                continue;

            var go = tilemap.GetInstantiatedObject(cell);
            if (go == null)
                continue;

            if (go.TryGetComponent<Node>(out var node))
            {
                node.Initialize(cell);
                nodes[cell] = node;
            }
            else if (go.TryGetComponent<EdgeNode>(out var edge))
            {
                edge.Initialize(cell);
                edges[cell] = edge;
            }
        }
    }



    public static Dictionary<Cardinal, EdgeNode> BuildEdgeMap(Node node, NodeManager nodeManager)
    {
        var edges = new Dictionary<Cardinal, EdgeNode>(4);

        foreach (var dir in CardinalUtil.Cardinals)
        {
            if (nodeManager.LookUpNeighbor(node, dir, out EdgeNode edge) && edge != null)
                edges[dir] = edge;
        }

        return edges;
    }
}
