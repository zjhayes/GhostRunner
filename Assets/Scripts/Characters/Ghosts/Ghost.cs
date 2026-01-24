using UnityEngine;

[RequireComponent(typeof(MovementManager))]
public class Ghost : CharacterManager
{
    [SerializeField] private int points = 200;
    [SerializeField] private GhostContext context;

    public int Points => points;
    public GhostContext Context => context;
    public Transform Target { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        context.Ghost = this;
    }

    private void Start()
    {
        ResetState();
        Target = GameManager.Instance.Player.transform;
    }

    protected override void HandleCollision(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.PLAYER))
        {
            if (context.Frightened.enabled)
            {
                GameManager.Instance.GhostEaten(this);
            }
            else
            {
                GameManager.Instance.PlayerEaten();
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(Layer.GHOSTS))
        {
            // Reverse direction on ghost/ghost bump.
            Movement.SetDirection(CardinalUtil.Opposite(Movement.Direction));
        }

        base.HandleCollision(other);
    }
}
