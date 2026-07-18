public enum EdgeTraversalResult
{
    Pass,
    Blocked,
    Interact,
    Teleport
}

public abstract class EdgeNode : Cell
{
    public virtual EdgeTraversalResult GetTraversalResult(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        return EdgeTraversalResult.Pass;
    }

    public abstract EdgeTraversalResult Resolve(
        CharacterManager character,
        Cardinal direction,
        Node node);
}
