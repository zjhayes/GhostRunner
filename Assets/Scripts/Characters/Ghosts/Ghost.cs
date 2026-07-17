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
            Ghost otherGhost = other.gameObject.GetComponent<Ghost>()
                ?? other.gameObject.GetComponentInParent<Ghost>();

            if (otherGhost != null)
                ScatterFrom(otherGhost);
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
        Context.EnterScatter();
        otherGhost.Context.EnterScatter();

        Vector2 separation = (Vector2)transform.position - (Vector2)otherGhost.transform.position;
        Cardinal away;

        if (Mathf.Abs(separation.x) > Mathf.Abs(separation.y))
            away = separation.x < 0f ? Cardinal.West : Cardinal.East;
        else if (Mathf.Abs(separation.y) > 0f)
            away = separation.y < 0f ? Cardinal.South : Cardinal.North;
        else
            away = GetInstanceID() < otherGhost.GetInstanceID() ? Cardinal.West : Cardinal.East;

        Movement.ApplyDirection(away);
        otherGhost.Movement.ApplyDirection(CardinalUtil.Opposite(away));
    }
}
