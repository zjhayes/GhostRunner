using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;

    public List<Vector2> AvailableDirections { get; private set; }

    private float castSize = 0.5f;
    private float castDistance = 1.0f;

    private void Awake()
    {
        AvailableDirections = new List<Vector2>();
    }

    private void Start()
    {
        CheckAvailableDirections();
    }

    private void CheckIfDirectionAvailable(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * castSize, 0.0f, direction, castDistance, obstacleLayer);
        
        if (hit.collider == null)
        {
            AvailableDirections.Add(direction);
        }
    }

    private void CheckAvailableDirections()
    {
        CheckIfDirectionAvailable(Vector2.up);
        CheckIfDirectionAvailable(Vector2.down);
        CheckIfDirectionAvailable(Vector2.left);
        CheckIfDirectionAvailable(Vector2.right);
    }
}
