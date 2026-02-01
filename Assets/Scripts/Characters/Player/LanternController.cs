using UnityEngine;

public class LanternController : MonoBehaviour
{
    [Header("Lantern Light")]
    [SerializeField] private Light lanternLight;
    [SerializeField] private LanternColor defaultLanternColor = LanternColor.DEFAULT;

    [Header("Lantern Sockets")]
    [SerializeField] private Transform eastSocket;
    [SerializeField] private Transform westSocket;
    [SerializeField] private Transform northSocket;
    [SerializeField] private Transform southSocket;

    [Header("Dependencies")]
    [SerializeField] private MovementManager movement;
    [SerializeField] private Animator animator;

    [Header("Run Swing")]
    [SerializeField] private float runSwingDistance = 0.15f;

    [Header("Walk Swing")]
    [SerializeField] private float walkSwingDistance = 0.04f;

    [Header("Phase Offset")]
    [SerializeField] private float phaseOffset = 0f;
    [SerializeField] private float offsetBlendSpeed = 20f;

    [Header("Run Hold Offsets")]
    [SerializeField] private Vector3 northRunOffset;
    [SerializeField] private Vector3 southRunOffset;
    [SerializeField] private Vector3 eastRunOffset;
    [SerializeField] private Vector3 westRunOffset;

    [Header("Walk Hold Offsets")]
    [SerializeField] private Vector3 northWalkOffset;
    [SerializeField] private Vector3 southWalkOffset;
    [SerializeField] private Vector3 eastWalkOffset;
    [SerializeField] private Vector3 westWalkOffset;

    private LanternColor currentColor;
    private Transform activeSocket;
    private Vector3 swingAxisLocal;
    private Vector3 runOffsetLocal;
    private Vector3 walkOffsetLocal;
    private Vector3 currentLocalOffset;
    private Vector3 targetLocalOffset;

    public Light Light => lanternLight;
    public LanternColor Color => currentColor;

    private enum LanternMotionState
    {
        Idle,
        Walk,
        Run
    }

    private void Awake()
    {
        if (!movement) movement = GetComponentInParent<MovementManager>();
        if (!lanternLight) lanternLight = GetComponentInChildren<Light>();
        if (!animator) animator = GetComponentInParent<Animator>();
        currentColor = defaultLanternColor;
        SetLanternColor(currentColor);
    }

    private void Start()
    {
        if (movement != null)
            ApplySocket(movement.Direction);
    }

    private void LateUpdate()
    {
        if (!movement || !lanternLight || !activeSocket || !animator)
            return;

        switch (ResolveState())
        {
            case LanternMotionState.Idle:
                targetLocalOffset = Vector3.zero;
                break;

            case LanternMotionState.Walk:
                ApplyAnimatorSwing(AnimationTag.WALK, walkOffsetLocal, walkSwingDistance);
                break;

            case LanternMotionState.Run:
                ApplyAnimatorSwing(AnimationTag.RUN, runOffsetLocal, runSwingDistance);
                break;
        }


        currentLocalOffset = Vector3.Lerp(
            currentLocalOffset,
            targetLocalOffset,
            Time.deltaTime * offsetBlendSpeed
        );

        lanternLight.transform.localPosition = currentLocalOffset;
    }

    public void SetLanternColor(LanternColor color)
    {
        if (!lanternLight)
            return;

        lanternLight.color = LanternColorUtil.ToColor(color);
        currentColor = color;
    }

    private LanternMotionState ResolveState()
    {
        if (!movement.IsMoving)
            return LanternMotionState.Idle;

        if (movement.SpeedMultiplier <= 1.5f)
            return LanternMotionState.Walk;

        return LanternMotionState.Run;
    }

    private void ApplyAnimatorSwing(string requiredTag, Vector3 baseOffset, float distance)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (!state.IsTag(requiredTag))
            return;

        float phase = (state.normalizedTime + phaseOffset) % 1f;

        // one-sided swing: center → max → center
        float swing01 = Mathf.Sin(phase * Mathf.PI);

        targetLocalOffset =
            baseOffset + swingAxisLocal * (swing01 * distance);
    }

    private void HandleDirectionChanged(Cardinal dir)
    {
        ApplySocket(dir);
    }

    private void ApplySocket(Cardinal dir)
    {
        Transform socket = dir switch
        {
            Cardinal.East => eastSocket,
            Cardinal.West => westSocket,
            Cardinal.North => northSocket,
            Cardinal.South => southSocket,
            _ => eastSocket
        };

        if (!socket || !lanternLight)
            return;

        activeSocket = socket;

        lanternLight.transform.SetParent(activeSocket, false);
        lanternLight.transform.localRotation = Quaternion.identity;

        swingAxisLocal = GetSwingAxisLocal(dir);
        runOffsetLocal = GetRunOffset(dir);
        walkOffsetLocal = GetWalkOffset(dir);

        currentLocalOffset = Vector3.zero;
        lanternLight.transform.localPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        if (movement != null)
            movement.OnDirectionChanged += HandleDirectionChanged;
    }

    private void OnDisable()
    {
        if (movement != null)
            movement.OnDirectionChanged -= HandleDirectionChanged;
    }

    private Vector3 GetSwingAxisLocal(Cardinal dir)
    {
        return dir switch
        {
            Cardinal.North => Vector3.right,
            Cardinal.South => Vector3.left,
            Cardinal.East => Vector3.down,
            Cardinal.West => Vector3.up,
            _ => Vector3.right
        };
    }

    private Vector3 GetRunOffset(Cardinal dir)
    {
        return dir switch
        {
            Cardinal.North => northRunOffset,
            Cardinal.South => southRunOffset,
            Cardinal.East => eastRunOffset,
            Cardinal.West => westRunOffset,
            _ => Vector3.zero
        };
    }

    private Vector3 GetWalkOffset(Cardinal dir)
    {
        return dir switch
        {
            Cardinal.North => northWalkOffset,
            Cardinal.South => southWalkOffset,
            Cardinal.East => eastWalkOffset,
            Cardinal.West => westWalkOffset,
            _ => Vector3.zero
        };
    }
}
