
public interface IGameManager : IService
{
    public PlayerManager Player { get; }
    public NodeManager NodeManager { get; }

}
