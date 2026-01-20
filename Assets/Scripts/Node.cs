using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Node : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;

    public List<Vector2> AvailableDirections { get; private set; }

    [Header("Direction Checks")]
    [SerializeField] float castSize = 0.5f;
    [SerializeField] float castDistance = 1.0f;

    private static readonly Vector2[] Cardinal =
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    private void Awake()
    {
        AvailableDirections = new List<Vector2>(4);
        gameObject.layer = LayerMask.NameToLayer(Layer.NODES);
    }

    private void Start()
    {
        CheckAvailableDirections();
    }

    private void CheckAvailableDirections()
    {
        AvailableDirections.Clear();

        for (int i = 0; i < Cardinal.Length; i++)
        {
            Vector2 dir = Cardinal[i];

            RaycastHit2D hit = Physics2D.BoxCast(
                transform.position,
                Vector2.one * castSize,
                0.0f,
                dir,
                castDistance,
                obstacleLayer
            );

            if (hit.collider == null)
            {
                AvailableDirections.Add(dir);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (AvailableDirections == null) return;

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireCube(transform.position, Vector3.one * castSize);

        foreach (var dir in AvailableDirections)
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)dir * 0.5f);
        }
    }
#endif
}
