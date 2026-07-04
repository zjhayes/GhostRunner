using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Ghost[] ghosts;
    [SerializeField] PlayerManager player;
    [SerializeField] NodeManager nodeManager;
    [SerializeField] Transform collectables;

    public Ghost[] Ghosts {  get { return ghosts; } }
    public PlayerManager Player { get { return player; } }
    public NodeManager NodeManager { get { return nodeManager; } }
    public Transform Collectables { get { return collectables; } }
    public int Lives { get; private set; }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (Lives < 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetLives(3);
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
        foreach (Transform collectable in Collectables)
        {
            collectable.gameObject.SetActive(true);
        }
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

    private void SetLives(int lives)
    {
        Lives = lives;
    }

    public void PlayerFrightened()
    {
        Player.Active(false);
        SetLives(Lives - 1);

        if (Lives > 0)
        {
            Invoke(nameof(NewRound), 3.0f);
        }
        else
        {
            GameOver();
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
