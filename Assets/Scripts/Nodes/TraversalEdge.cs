
public class TraversalEdge : EdgeNode
{
    public override void Resolve(MovementManager movement, Cardinal direction, Node node)
    {
        movement.ApplyDirection(direction);
    }
}
