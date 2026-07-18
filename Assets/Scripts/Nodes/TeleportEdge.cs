using UnityEngine;

public class TeleportEdge : EdgeNode
{
    [SerializeField] private int teleportId;

    private Node targetNode;

    public int TeleportId => teleportId;

    private void Start()
    {
        //targetNode = NodeUtil.FindNodeByTeleportId(this, teleportId);
    }

    public override EdgeTraversalResult GetTraversalResult(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        return targetNode == null
            ? EdgeTraversalResult.Blocked
            : EdgeTraversalResult.Teleport;
    }

    public override EdgeTraversalResult Resolve(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        if (targetNode == null)
        {
            Debug.LogError($"TeleportEdge {name} not bound!");
            return EdgeTraversalResult.Blocked;
        }

        //movement.TeleportToNode(targetNode);
        return EdgeTraversalResult.Teleport;
    }
}

