using UnityEngine;

[RequireComponent(typeof(MovementManager))]
public class Ghost : CharacterManager
{
    [SerializeField] private GhostContext context;

    private bool hasPlayerContact;

    public GhostContext Context => context;
    public Transform Target { get; private set; }

    protected virtual void Awake()
    {
        context.Ghost = this;
    }

    protected override void Start()
    {
        base.Start();
        Movement.Rigidbody.useFullKinematicContacts = true;
        Target = GameManager.Player.transform;
        ResetState();
    }

    public override void ResetState()
    {
        hasPlayerContact = false;
        base.ResetState();
    }

    protected override void HandleCollision(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.PLAYER))
        {
            StopAndFacePlayer(other.transform, "collision");
            GameManager.Player.PlayerFrightened();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(Layer.GHOSTS))
        {
            HandleGhostContact(other.gameObject);
        }

        base.HandleCollision(other);
    }

    protected override void HandleTrigger(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.PLAYER))
        {
            StopAndFacePlayer(other.transform, "trigger");
            GameManager.Player.PlayerFrightened();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(Layer.GHOSTS))
        {
            HandleGhostContact(other.gameObject);
        }

        base.HandleTrigger(other);
    }

    public void StopAndFacePlayer(Transform contactTransform, string contactType)
    {
        if (hasPlayerContact)
            return;

        hasPlayerContact = true;

        Transform player = Target != null ? Target : contactTransform;
        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        Cardinal facingDirection = CardinalUtil.FromVector(toPlayer, Movement.Direction);

        Movement.ApplyDirection(facingDirection);
        Context.EnterIdle();
    }

    public void TurnAround()
    {
        Cardinal newDirection = CardinalUtil.Opposite(Movement.Direction);
        Movement.SetDirection(newDirection);
    }

    private void ScatterFrom(Ghost otherGhost)
    {
        if (Context.IsIdle || otherGhost.Context.IsIdle)
            return;

        Context.EnterScatter();
        otherGhost.Context.EnterScatter();

        Vector2 separation = (Vector2)transform.position - (Vector2)otherGhost.transform.position;
        TurnAwayFrom(Movement, separation);
        TurnAwayFrom(otherGhost.Movement, -separation);
    }

    private static void TurnAwayFrom(MovementManager movement, Vector2 away)
    {
        Vector2 forward = CardinalUtil.ToVector(movement.Direction);
        if (Vector2.Dot(forward, away) < 0f)
            movement.SetDirection(CardinalUtil.Opposite(movement.Direction));
    }

    private void HandleGhostContact(GameObject other)
    {
        Ghost otherGhost = other.GetComponent<Ghost>()
            ?? other.GetComponentInParent<Ghost>();

        if (otherGhost != null && otherGhost != this)
            ScatterFrom(otherGhost);
    }
}
