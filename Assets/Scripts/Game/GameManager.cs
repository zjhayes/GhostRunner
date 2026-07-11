using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{
    private const string GameOverScene = "GameOver";

    [SerializeField] Ghost[] ghosts;
    [SerializeField] PlayerManager player;
    [SerializeField] NodeManager nodeManager;

    public Ghost[] Ghosts {  get { return ghosts; } }
    public PlayerManager Player { get { return player; } }
    public NodeManager NodeManager { get { return nodeManager; } }

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
    }

    private void OnDestroy()
    {
        if (Player?.Fear != null)
            Player.Fear.OnFrightened -= GameOver;
    }

    private void GameOver()
    {
        SceneManager.LoadScene(GameOverScene);
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
