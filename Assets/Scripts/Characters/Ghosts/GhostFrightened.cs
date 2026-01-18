
using UnityEngine;

public class GhostFrightened : GhostBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    public bool eaten { get; private set; }

    private void Start()
    {
        Context.Ghost.Movement.SpeedMultiplier = 0.5f;
        Context.Ghost.OnCollisionEntered += OnCollision;
        Context.Ghost.OnTriggerEnter += OnTrigger;
    }


    public override void Enable(float duration)
    {
        base.Enable(duration);

        body.enabled = false;
        eyes.enabled = false;
        blue.enabled = true;
        white.enabled = false;

        Invoke(nameof(Flash), duration * Numeric.HALF);
    }

    public override void Disable()
    {
        base.Disable();

        body.enabled = true;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }

    private void Flash()
    {
        if (!eaten) return;

        blue.enabled = false;
        white.enabled = true;
        white.GetComponent<SpriteAnimator>().Restart();
    }

    private void Eaten()
    {
        eaten = true;

        Vector3 position = Context.HomeTransform.position;
        position.z = Context.Ghost.transform.position.z;
        Context.Ghost.transform.position = position;

        Context.Home.Enable(duration);

        body.enabled = false;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }

    private void OnCollision(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.PLAYER))
        {
            if (enabled)
            {
                Eaten();
            }
        }
    }

    private void OnDisable()
    {
        Context.Ghost.Movement.SpeedMultiplier = 1.0f;
        Context.Ghost.OnCollisionEntered -= OnCollision;
        Context.Ghost.OnTriggerEnter -= OnTrigger;
        eaten = false;
    }

    private void OnTrigger(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (enabled && node != null)
        {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            foreach (Vector2 availableDirection in node.AvailableDirections)
            {
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y, transform.position.z);
                float distance = (Context.Ghost.Target.position - newPosition).sqrMagnitude;

                if (distance > maxDistance)
                {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            Context.Ghost.Movement.SetDirection(direction);
        }
    }
}
