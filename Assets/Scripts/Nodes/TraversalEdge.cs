
public class TraversalEdge : EdgeNode
{
    public override void Resolve(CharacterManager character, Cardinal direction, Node node)
    {
        character.Movement.ApplyDirection(direction);
    }
}
