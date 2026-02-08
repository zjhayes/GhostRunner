using UnityEngine;

public class TorchAction : NodeAction
{
    [SerializeField] Torch torch;

    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
        if(edge.ActionType != ActionType.TORCH)
            return;

        // Look at the torch.
        player.Movement.ApplyDirection(direction);
        player.Movement.Stop();

        player.Lantern.SetLanternColor(torch.LightColor);
    }

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
        /*Cardinal[] exclude = { direction };

        if (ghost.Movement.TryResolveDirection(node, out var chosen, exclude))
        {
            node.ResolveEdge(ghost, chosen);
        }
        else
        {
            // No valid alternatives; pick your fallback.
            ghost.Movement.Stop();
        }*/

        ghost.TurnAround();
    }
}
