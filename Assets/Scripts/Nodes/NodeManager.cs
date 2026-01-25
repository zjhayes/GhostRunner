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
}
