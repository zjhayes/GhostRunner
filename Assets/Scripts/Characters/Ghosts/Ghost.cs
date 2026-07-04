using UnityEngine;

[RequireComponent(typeof(MovementManager))]
public class Ghost : CharacterManager
{
    [SerializeField] private GhostContext context;

    public GhostContext Context => context;
    public Transform Target { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        context.Ghost = this;
    }

    protected override void Start()
    {
        base.Start();
        ResetState();
        Target = GameManager.Instance.Player.transform;
    }

    protected override void HandleCollision(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.PLAYER))
        {
            GameManager.Instance.PlayerFrightened();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(Layer.GHOSTS))
        {
            // Reverse direction on ghost/ghost bump.
            Movement.SetDirection(CardinalUtil.Opposite(Movement.Direction));
        }

        base.HandleCollision(other);
    }

    public void TurnAround()
    {
        Cardinal newDirection = CardinalUtil.Opposite(Movement.Direction);
        Movement.SetDirection(newDirection);
    }
}
