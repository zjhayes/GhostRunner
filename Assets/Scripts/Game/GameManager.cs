using System;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    [SerializeField] Ghost[] ghosts;
    [SerializeField] PlayerManager player;
    [SerializeField] NodeManager nodeManager;
    [SerializeField] Checkpoint checkpoint;
    [SerializeField] UIManager uiManager;
    [SerializeField] SceneController sceneController;

    public Ghost[] Ghosts {  get { return ghosts; } }
    public PlayerManager Player { get { return player; } }
    public NodeManager NodeManager { get { return nodeManager; } }
    public Checkpoint Checkpoint { get { return checkpoint; } }
    public UIManager UI { get { return uiManager; } }
    public SceneController Scene { get { return sceneController; } }

    public event Action OnGameOver;

    void Awake()
    {
        // Inject gameManager into dependents.
        ServiceInjector.Resolve<IGameManager, GameBehaviour>(this);
    }

    private void Start()
    {
        Player.Fear.OnFrightened += GameOver;
        ResetGhosts();
        Player.ResetState();
        checkpoint?.Restore(Player);
    }

    private void OnDestroy()
    {
        if (Player?.Fear != null)
            Player.Fear.OnFrightened -= GameOver;
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
    }

    private void ResetGhosts()
    {
        for (int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].ResetState();
        }
    }

    public void OnCollect(Collectable collectable)
    {
        collectable.gameObject.SetActive(false);
    }
}
