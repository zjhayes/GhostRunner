using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private MovementManager movement;

    [Header("Shadow Blob")]
    [SerializeField] private Transform shadowBlob;
    [SerializeField] private SpriteRenderer shadowRenderer;

    [Header("Per-Direction Offsets (Local XY)")]
    [SerializeField] private Vector2 eastOffset;
    [SerializeField] private Vector2 westOffset;
    [SerializeField] private Vector2 northOffset;
    [SerializeField] private Vector2 southOffset;

    [Header("Per-Direction Rotation (Local Z, Degrees)")]
    [SerializeField] private float eastRotationZ = 0f;
    [SerializeField] private float westRotationZ = 0f;
    [SerializeField] private float northRotationZ = 90f;
    [SerializeField] private float southRotationZ = 90f;

    [Header("Per-Direction Flips")]
    [SerializeField] private bool eastFlipX = false;
    [SerializeField] private bool eastFlipY = true;

    [SerializeField] private bool westFlipX = true;
    [SerializeField] private bool westFlipY = false;

    [SerializeField] private bool northFlipX = false;
    [SerializeField] private bool northFlipY = true;

    [SerializeField] private bool southFlipX = true;
    [SerializeField] private bool southFlipY = false;

    [Header("Tuning")]
    [SerializeField] private bool flipUsingScale = true;
    [SerializeField] private bool useLastFacingWhenIdle = true;

    private Cardinal facing = Cardinal.East;
    private Vector3 baseLocalPos;
    private Vector3 baseLocalScale;

    private void Awake()
    {
        if (!movement) movement = GetComponentInParent<MovementManager>();

        if (!shadowBlob) shadowBlob = transform;
        if (!shadowRenderer) shadowRenderer = shadowBlob.GetComponent<SpriteRenderer>();

        baseLocalPos = shadowBlob.localPosition;
        baseLocalScale = shadowBlob.localScale;
    }

    private void OnEnable()
    {
        if (movement == null)
        {
            Debug.LogWarning("ShadowController: MovementManager not assigned; shadow will never update.");
            return;
        }

        movement.OnDirectionChanged += HandleDirectionChanged;
    }

    private void OnDisable()
    {
        if (movement == null) return;
        movement.OnDirectionChanged -= HandleDirectionChanged;
    }

    private void Start()
    {
        if (movement != null)
            facing = movement.Direction;

        ApplyFacing(facing);
    }

    private void HandleDirectionChanged(Cardinal dir)
    {
        if (dir == facing && useLastFacingWhenIdle)
            return;

        facing = dir;
        ApplyFacing(facing);
    }

    private void ApplyFacing(Cardinal dir)
    {
        // Offset.
        Vector2 offset = dir switch
        {
            Cardinal.East  => eastOffset,
            Cardinal.West  => westOffset,
            Cardinal.North => northOffset,
            Cardinal.South => southOffset,
            _ => Vector2.zero
        };

        shadowBlob.localPosition = new Vector3(
            baseLocalPos.x + offset.x,
            baseLocalPos.y + offset.y,
            baseLocalPos.z // <-- Z unchanged
        );

        // Rotation.
        float zRot = dir switch
        {
            Cardinal.East  => eastRotationZ,
            Cardinal.West  => westRotationZ,
            Cardinal.North => northRotationZ,
            Cardinal.South => southRotationZ,
            _ => 0f
        };

        ApplyRotationZ(shadowBlob, zRot);

        // Flips.
        bool flipX, flipY;
        switch (dir)
        {
            case Cardinal.East:  flipX = eastFlipX;  flipY = eastFlipY;  break;
            case Cardinal.West:  flipX = westFlipX;  flipY = westFlipY;  break;
            case Cardinal.North: flipX = northFlipX; flipY = northFlipY; break;
            case Cardinal.South: flipX = southFlipX; flipY = southFlipY; break;
            default:             flipX = false;     flipY = false;      break;
        }

        ApplyFlips(shadowBlob, shadowRenderer, baseLocalScale, flipX, flipY, flipUsingScale);
    }

    private static void ApplyRotationZ(Transform t, float zRot)
    {
        var e = t.localEulerAngles;
        e.z = zRot;
        t.localEulerAngles = e;
    }

    private static void ApplyFlips(
        Transform t,
        SpriteRenderer r,
        Vector3 baseScale,
        bool flipX,
        bool flipY,
        bool flipUsingScale)
    {
        if (!flipUsingScale)
        {
            if (!r)
            {
                Debug.LogWarning("ShadowController: flipUsingScale is false but no SpriteRenderer is assigned/found.");
                return;
            }

            r.flipX = flipX;
            r.flipY = flipY;
            return;
        }

        // Reset to base scale then apply sign flips.
        var s = baseScale;
        s.x = Mathf.Abs(s.x) * (flipX ? -1f : 1f);
        s.y = Mathf.Abs(s.y) * (flipY ? -1f : 1f);
        t.localScale = s;
    }
}