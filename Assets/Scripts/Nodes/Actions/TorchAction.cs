using UnityEngine;

public class TorchAction : NodeAction
{
    [SerializeField] Torch torch;

    protected override EdgeTraversalResult GetPlayerTraversalResult(
        PlayerManager player,
        Cardinal direction,
        Node node,
        ActionEdge edge)
    {
        return EdgeTraversalResult.Interact;
    }

    protected override EdgeTraversalResult GetGhostTraversalResult(
        Ghost ghost,
        Cardinal direction,
        Node node,
        ActionEdge edge)
    {
        return EdgeTraversalResult.Blocked;
    }

    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
        if(edge.ActionType != ActionType.TORCH)
            return;

        player.Lantern.SetLanternColor(torch.LightColor);
    }

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
    }
}
