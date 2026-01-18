using UnityEngine;

public class GhostScatter : GhostBehaviour
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
            int index = Random.Range(0, node.AvailableDirections.Count);
            
            if (node.AvailableDirections[index] == -Context.Ghost.Movement.Direction && node.AvailableDirections.Count > 1)
            {
                index++;

                if (index >= node.AvailableDirections.Count)
                {
                    index = 0;
                }
            }

            Context.Ghost.Movement.SetDirection(node.AvailableDirections[index]);
        }
    }

    private void OnDisable()
    {
        Context.Chase.Enable();
    }
}
