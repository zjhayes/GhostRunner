public class CheckpointEdge : TraversalEdge
{
    public override void Resolve(CharacterManager character, Cardinal direction, Node node)
    {
        base.Resolve(character, direction, node);

        if (character is PlayerManager)
            GameManager.Checkpoint?.Activate(node, direction);
    }
}
