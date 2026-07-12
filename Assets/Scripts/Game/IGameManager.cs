
using System;

public interface IGameManager : IService
{
    public PlayerManager Player { get; }
    public NodeManager NodeManager { get; }
    public Checkpoint Checkpoint { get; }
    public UIManager UI { get; }
    public SceneController Scene { get; }
    public event Action OnGameOver;

}
