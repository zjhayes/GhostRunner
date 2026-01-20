using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Node : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;

    public HashSet<Cardinal> AvailableDirections { get; private set; }

    [Header("Direction Checks")]
    [SerializeField] private float castSize = 0.5f;
    [SerializeField] private float castDistance = 1.0f;

    private static readonly Cardinal[] Cardinals =
    {
        Cardinal.North,
        Cardinal.South,
        Cardinal.West,
        Cardinal.East
    };

    private void Awake()
    {
        AvailableDirections = new HashSet<Cardinal>();
        gameObject.layer = LayerMask.NameToLayer(Layer.NODES);
    }

    private void Start()
    {
        RefreshAvailableDirections();
    }

    public void RefreshAvailableDirections()
    {
        AvailableDirections.Clear();

        foreach (var c in Cardinals)
        {
            Vector2 dir = CardinalUtil.ToVector(c);

            RaycastHit2D hit = Physics2D.BoxCast(
                transform.position,
                Vector2.one * castSize,
                0f,
                dir,
                castDistance,
                obstacleLayer
            );

            if (hit.collider == null)
            {
                AvailableDirections.Add(c);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // In editor, show the cast box and available exits.
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireCube(transform.position, Vector3.one * castSize);

        // If we haven't played yet, approximate directions for visualization.
        IEnumerable<Cardinal> dirs = AvailableDirections ?? (IEnumerable<Cardinal>)Cardinals;

        foreach (var c in dirs)
        {
            Vector3 dir = (Vector3)CardinalUtil.ToVector(c);
            Gizmos.DrawLine(transform.position, transform.position + dir * 0.5f);
        }
    }
#endif
}
