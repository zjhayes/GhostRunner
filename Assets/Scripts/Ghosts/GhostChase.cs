
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

        if (node != null && enabled && !Context.Frightened.enabled)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach (Vector2 availableDirection in node.AvailableDirections)
            {
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y, transform.position.z);
                float distance = (Context.Ghost.Target.position - newPosition).sqrMagnitude;

                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            Context.Ghost.Movement.SetDirection(direction);
        }
    }

    private void OnDisable()
    {
        Context.Ghost.OnTriggerEnter -= OnTrigger;
        Context.Scatter.Enable();
    }
}
