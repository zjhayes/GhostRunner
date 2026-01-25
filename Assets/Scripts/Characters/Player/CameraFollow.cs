using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;

    [Header("Follow")]
    [SerializeField] Vector3 offset = new Vector3(0f, 8f, -8f);
    [SerializeField] float smoothTime = 0.15f;
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;
    [SerializeField] bool followZ = true;

    [Header("Optional Bounds")]
    [SerializeField] bool useBounds = false;
    [SerializeField] Vector3 minBounds = new Vector3(-100f, -100f, -100f);
    [SerializeField] Vector3 maxBounds = new Vector3(100f, 100f, 100f);

    private Vector3 velocity;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;

        // Keep axes you don't want to follow at current camera position
        Vector3 current = transform.position;
        if (!followX) desired.x = current.x;
        if (!followY) desired.y = current.y;
        if (!followZ) desired.z = current.z;

        Vector3 smoothed = Vector3.SmoothDamp(current, desired, ref velocity, smoothTime);

        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minBounds.x, maxBounds.x);
            smoothed.y = Mathf.Clamp(smoothed.y, minBounds.y, maxBounds.y);
            smoothed.z = Mathf.Clamp(smoothed.z, minBounds.z, maxBounds.z);
        }

        transform.position = smoothed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;
        Gizmos.color = Color.yellow;
        Vector3 center = (minBounds + maxBounds) * 0.5f;
        Vector3 size = (maxBounds - minBounds);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
