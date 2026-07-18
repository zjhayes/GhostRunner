public class CheckpointEdge : TraversalEdge
{
    public override EdgeTraversalResult Resolve(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        EdgeTraversalResult result = base.Resolve(character, direction, node);

        if (character is PlayerManager)
            GameManager.Checkpoint?.Activate(node, direction);

        return result;
    }
}
