using UnityEngine;

public class TorchAction : NodeAction
{
    [SerializeField] Torch torch;

    public override void OnResolve(CharacterManager character, Cardinal direction, Node node, ActionEdge edge)
    {
        if(edge.ActionType != ActionType.TORCH)
            return;

        character.Movement.ApplyDirection(direction);
        character.Movement.Stop();

        if (character is not PlayerManager player)
            return;

        player.Lantern.SetLanternColor(torch.LightColor);
    }
}
