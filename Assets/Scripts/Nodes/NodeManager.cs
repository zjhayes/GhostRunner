using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeManager : MonoBehaviour
{
    [SerializeField] private Tilemap nodeTilemap;

    private Dictionary<Vector3Int, Node> nodeTiles;
    private Dictionary<Vector3Int, EdgeNode> edgeTiles;

    public Tilemap NodeTilemap => nodeTilemap;
    public Dictionary<Vector3Int, Node> Nodes => nodeTiles;
    public Dictionary<Vector3Int, EdgeNode> Edges => edgeTiles;

    void Awake()
    {
        NodeUtil.GetAllNodes(nodeTilemap, out nodeTiles, out edgeTiles);
    }
    
    public bool LookUpNeighbor(Node node, Cardinal dir, out EdgeNode edge)
    {
        edge = null;

        if (node == null)
            return false;

        Vector3Int cell = node.Position + CardinalUtil.ToCellOffset(dir);
        return edgeTiles.TryGetValue(cell, out edge);
    }

    public bool TryGetEdgeAtPosition<T>(
        Vector3 worldPosition,
        Vector3 worldOffset,
        out T result
    ) where T : EdgeNode
    {
        result = null;

        Vector3Int cell = WorldToCell(worldPosition + worldOffset);

        if (edgeTiles.TryGetValue(cell, out var edge) && edge is T typedEdge)
        {
            result = typedEdge;
            return true;
        }

        return false;
    }


    public bool TryFindEdgeInDirection<T>(
        Vector3 worldPosition,
        Vector3 worldOffset,
        Cardinal direction,
        int distance,
        out T result
    ) where T : EdgeNode
    {
        result = null;

        Vector3Int startCell = WorldToCell(worldPosition + worldOffset);

        Vector3Int offset = CardinalUtil.ToCellOffset(direction);

        for (int i = 1; i <= distance; i++)
        {
            Vector3Int cell = startCell + offset * i;

            if (edgeTiles.TryGetValue(cell, out var edge) && edge is T typedEdge)
            {
                result = typedEdge;
                return true;
            }
        }

        return false;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return nodeTilemap.WorldToCell(worldPosition);
    }


    public bool TryFindActionEdge(
        Vector3 worldPosition,
        Vector3 worldOffset,
        Cardinal direction,
        int distance,
        out ActionEdge actionEdge
    )
    {
        return TryFindEdgeInDirection(
            worldPosition,
            worldOffset,
            direction,
            distance,
            out actionEdge
        );
    }



}
