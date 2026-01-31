using UnityEngine;

[ExecuteAlways]
public class RainBoundary : MonoBehaviour
{
    [SerializeField] private Vector3 size = new Vector3(20f, 20f, 20f);

    public Bounds WorldBounds => new Bounds(transform.position, size);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.25f);
        Gizmos.DrawCube(transform.position, size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, size);
    }
#endif
}
