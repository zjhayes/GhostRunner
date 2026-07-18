
public class TraversalEdge : EdgeNode
{
    public override EdgeTraversalResult Resolve(
        CharacterManager character,
        Cardinal direction,
        Node node)
    {
        return EdgeTraversalResult.Pass;
    }
}
