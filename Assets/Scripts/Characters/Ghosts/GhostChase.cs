using UnityEngine;

public class GhostChase : GhostBehaviour
{
    private void Start()
    {
        Context.Ghost.OnTriggerEnter += OnTrigger;
    }

    private void OnTrigger(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        if (node == null) return;

        if (!enabled || Context.Frightened.enabled) return;

        Cardinal? best = null;
        float minDistance = float.MaxValue;

        foreach (Cardinal available in node.AvailableDirections)
        {
            Vector2 step = CardinalUtil.ToVector(available);

            // Predict the next tile/step in that direction.
            Vector3 newPosition = transform.position + new Vector3(step.x, step.y, 0f);

            float distance = (Context.Ghost.Target.position - newPosition).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                best = available;
            }
        }

        if (best.HasValue)
            Context.Ghost.Movement.SetDirection(best.Value);
    }

    private void OnDisable()
    {
        Context.Ghost.OnTriggerEnter -= OnTrigger;
        Context.Scatter.Enable();
    }
}
