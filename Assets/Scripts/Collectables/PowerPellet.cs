
public class PowerPellet : Collectable
{
    public float Duration { get; private set; } = 8.0f;

    protected override void Collect()
    {
        base.Collect();
        PowerPelletCollected();
    }


    public void PowerPelletCollected()
    {
        foreach (Ghost ghost in GameManager.Instance.Ghosts)
        {
            ghost.Context.Frightened.Enable(Duration);
        }

        CancelInvoke();
        Invoke(nameof(GameManager.Instance.ResetGhostMultiplier), Duration);
    }
}
