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
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int GhostMultiplier { get; private set; } = 1;

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
        SetScore(0);
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
        ResetGhostMultiplier();
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
            //Ghosts[i].ResetState();
        }
    }

    private void DisableGhosts()
    {
        for (int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].Active(false);
        }
    }

    private void SetScore(int score)
    {
        Score = score;
    }

    private void SetLives(int lives)
    {
        Lives = lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(Score + (ghost.Points * GhostMultiplier));
        GhostMultiplier++;
    }

    public void PlayerEaten()
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

        SetScore(Score + collectable.Points);

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

    public void ResetGhostMultiplier()
    {
        GhostMultiplier = 1;
    }    
}
