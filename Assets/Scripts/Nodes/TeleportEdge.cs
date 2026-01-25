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

    public override void Resolve(MovementManager movement, Cardinal direction, Node node)
    {
        if (targetNode == null)
        {
            Debug.LogError($"TeleportEdge {name} not bound!");
            return;
        }

        //movement.TeleportToNode(targetNode);
    }
}

