using UnityEngine;

public class ChangeMusicAction : NodeAction
{
    [SerializeField] private AudioClip track;

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
        // Ghosts do not change the music.
    }

    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
        if (edge.ActionType == ActionType.CHANGE_MUSIC)
            GameManager.Music?.ChangeTrack(track);
    }
}
