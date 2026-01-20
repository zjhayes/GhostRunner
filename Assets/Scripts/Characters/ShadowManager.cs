using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    [SerializeField] private MovementManager movement;

    [Header("Shadow Blob")]
    [SerializeField] private Transform shadowBlob;
    [SerializeField] private SpriteRenderer shadowRenderer;

    [Header("Tuning")]
    [SerializeField] private bool flipUsingScale = true;
    [SerializeField] private bool useLastFacingWhenIdle = true;

    private Cardinal facing = Cardinal.East;

    private void Awake()
    {
        if (!shadowBlob) shadowBlob = transform;
        if (!shadowRenderer) shadowRenderer = shadowBlob.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (movement != null)
            movement.OnDirectionChanged += HandleDirectionChanged;
        else
            Debug.LogWarning("ShadowManager: MovementManager not assigned; shadow will never update.");
    }

    private void OnDisable()
    {
        if (movement != null)
            movement.OnDirectionChanged -= HandleDirectionChanged;
    }

    private void Start()
    {
        ApplyFacing(facing);
    }

    private void HandleDirectionChanged(Vector2 dir)
    {
        if (dir == Vector2.zero && useLastFacingWhenIdle)
            return;

        var next = CardinalUtil.FromVector(dir, facing);
        if (next == facing) return;

        facing = next;
        ApplyFacing(facing);
    }

private void ApplyFacing(Cardinal dir)
{
    float zRot;
    bool flipX;
    bool flipY;

    switch (dir)
    {
        case Cardinal.East:
            zRot = 0f;   flipX = false; flipY = true;
            break;

        case Cardinal.West:
            zRot = 0f;   flipX = true;  flipY = false;
            break;

        case Cardinal.North:
            zRot = 90f;  flipX = false; flipY = true;
            break;

        case Cardinal.South:
            zRot = 90f;  flipX = true;  flipY = false;
            break;

        default:
            zRot = 0f;   flipX = false; flipY = false;
            break;
    }

    ApplyRotation(shadowBlob, zRot);
    ApplyFlips(shadowBlob, shadowRenderer, flipX, flipY, flipUsingScale);
}


    private static void ApplyRotation(Transform t, float zRot)
    {
        var e = t.localEulerAngles;
        e.z = zRot;
        t.localEulerAngles = e;
    }

    private static void ApplyFlips(Transform t, SpriteRenderer r, bool flipX, bool flipY, bool flipUsingScale)
    {
        if (!flipUsingScale)
        {
            if (!r)
            {
                Debug.LogWarning("ShadowManager: flipUsingScale is false but no SpriteRenderer is assigned/found.");
                return;
            }

            r.flipX = flipX;
            r.flipY = flipY;
            return;
        }

        var s = t.localScale;
        s.x = Mathf.Abs(s.x) * (flipX ? -1f : 1f);
        s.y = Mathf.Abs(s.y) * (flipY ? -1f : 1f);
        t.localScale = s;
    }
}
