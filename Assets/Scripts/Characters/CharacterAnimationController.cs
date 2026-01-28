using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private MovementManager movementManager;

    private Animator animator;
    private float runSpeedThreshold = 1.5f;

    private static readonly int DirX = Animator.StringToHash(AnimatorProperty.DIRECTION_X);
    private static readonly int DirY = Animator.StringToHash(AnimatorProperty.DIRECTION_Y);
    private static readonly int Moving = Animator.StringToHash(AnimatorProperty.IS_MOVING);
    private static readonly int Running = Animator.StringToHash(AnimatorProperty.IS_RUNNING);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (!movementManager) movementManager = GetComponentInParent<MovementManager>();
    }

    private void OnEnable()
    {
        if (movementManager == null) return;
        
        movementManager.OnDirectionChanged += UpdateDirection;
        movementManager.OnMovingChanged += UpdateMoving;
        movementManager.OnSpeedChanged += isRunning => UpdateRunning(isRunning > runSpeedThreshold);

        UpdateMoving(movementManager.IsMoving);
        UpdateDirection(movementManager.Direction);
        UpdateRunning(movementManager.SpeedMultiplier > runSpeedThreshold);
    }

    private void OnDisable()
    {
        if (movementManager == null) return;

        movementManager.OnDirectionChanged -= UpdateDirection;
        movementManager.OnMovingChanged -= UpdateMoving;
        movementManager.OnSpeedChanged -= isRunning => UpdateRunning(isRunning > runSpeedThreshold);
    }

    private void UpdateDirection(Cardinal dir)
    {
        Vector2 v = CardinalUtil.ToVector(dir);
        animator.SetFloat(DirX, v.x);
        animator.SetFloat(DirY, v.y);
    }

    private void UpdateMoving(bool isMoving)
    {
        animator.SetBool(Moving, isMoving);
    }

    private void UpdateRunning(bool isRunning)
    {
        animator.SetBool(Running, isRunning);
    }
}
