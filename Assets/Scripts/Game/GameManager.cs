using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    [SerializeField] Ghost[] ghosts;
    [SerializeField] PlayerManager player;
    [SerializeField] NodeManager nodeManager;
    [SerializeField] Transform collectables;

    public Ghost[] Ghosts {  get { return ghosts; } }
    public PlayerManager Player { get { return player; } }
    public NodeManager NodeManager { get { return nodeManager; } }
    public Transform Collectables { get { return collectables; } }

    void Awake()
    {
        // Inject gameManager into dependents.
        ServiceInjector.Resolve<IGameManager, GameBehaviour>(this);
    }

    private void Start()
    {
        NewGame();
    }

    /*private void Update()
    {
        if (Lives < 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }*/

    private void NewGame()
    {
        NewScene();
    }

    private void NewScene()
    {
        ResetCollectables();
        NewRound();
    }

    private void NewRound()
    {
        ResetGhosts();
        Player.ResetState();
    }

    private void GameOver()
    {
        DisableGhosts();
        Player.Active(false);
        ResetCollectables();
    }

    private void ResetCollectables()
    {
        /*foreach (Transform collectable in Collectables)
        {
            collectable.gameObject.SetActive(true);
        }*/
    }

    private void ResetGhosts()
    {
        for (int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].ResetState();
        }
    }

    private void DisableGhosts()
    {
        for (int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].Active(false);
        }
    }

    public void OnCollect(Collectable collectable)
    {
        collectable.gameObject.SetActive(false);

        if (!HasRemainingCollectables())
        {
            Player.Active(false);
            Invoke(nameof(NewScene), 3.0f);
        }
    }

    private bool HasRemainingCollectables()
    {
        foreach (Transform collectable in Collectables)
        {
            if (collectable.gameObject.activeSelf) return true;
        }
        return false;
    }
}
