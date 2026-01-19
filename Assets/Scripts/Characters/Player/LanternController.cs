using UnityEngine;

public class LanternController : MonoBehaviour
{
    [SerializeField] MovementManager movement;
    [SerializeField] Light lanternLight;
    [SerializeField] Transform eastLanternTransform;
    [SerializeField] Transform westLanternTransform;
    [SerializeField] Transform northLanternTransform;
    [SerializeField] Transform southLanternTransform;

    private void HandleDirectionChanged(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        // If you ever pass diagonals, resolve to a cardinal direction.
        dir = Conversion.QuantizeToCardinal(dir);

        ApplySocket(dir);
    }

    private void ApplySocket(Vector2 dir)
    {
        Transform socket = GetSocketFor(dir);
        if (socket == null) return;

        Transform t = lanternLight.transform;
        if (t == null) return;

        t.position = socket.position;
        t.rotation = socket.rotation;
    }

    private Transform GetSocketFor(Vector2 dir)
    {
        if (dir.x > 0.5f) return eastLanternTransform;
        if (dir.x < -0.5f) return westLanternTransform;
        if (dir.y > 0.5f) return northLanternTransform;
        return southLanternTransform;
    }

    private void OnEnable()
    {
        if (movement == null) return;
        movement.OnDirectionChanged += HandleDirectionChanged;
    }

    private void OnDisable()
    {
        if (movement == null) return;
        movement.OnDirectionChanged -= HandleDirectionChanged;
    }
}
