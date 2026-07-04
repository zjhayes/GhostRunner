using System.Collections.Generic;
using UnityEngine;

public class GhostFrightened : GhostBehaviour
{
    public bool eaten { get; private set; }

    private void Start()
    {
        Context.Ghost.Movement.SpeedMultiplier = 0.5f;
        Context.Ghost.OnCollisionEntered += OnCollision;
    }

    public override void Enable(float duration)
    {
        base.Enable(duration);

        Invoke(nameof(Flash), duration * Numeric.HALF);
    }

    public override void Disable()
    {
        base.Disable();
    }

    private void Flash()
    {
        // Flash only if NOT eaten (typical Pac-Man behavior)
        if (eaten) return;
    }

    private void Eaten()
    {
        eaten = true;

        Vector3 position = Context.HomeTransform.position;
        position.z = Context.Ghost.transform.position.z;
        Context.Ghost.transform.position = position;

        Context.Home.Enable(duration);
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

    protected override void OnDisable()
    {
        base.OnDisable();
        Context.Ghost.Movement.SpeedMultiplier = 1.0f;
        Context.Ghost.OnCollisionEntered -= OnCollision;
        eaten = false;
    }

    protected override bool TryPickDirection(Node node, IReadOnlyList<Cardinal> candidates, out Cardinal chosen)
    {
        chosen = default;
        if (candidates.Count == 0)
            return false;

        bool hasBest = false;
        float maxDistance = float.MinValue;

        foreach (Cardinal available in candidates)
        {
            Vector2 step = CardinalUtil.ToVector(available);
            Vector3 newPosition = node.transform.position + new Vector3(step.x, step.y, 0f);

            float distance = (Context.Ghost.Target.position - newPosition).sqrMagnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                chosen = available;
                hasBest = true;
            }
        }

        return hasBest;
    }
}
