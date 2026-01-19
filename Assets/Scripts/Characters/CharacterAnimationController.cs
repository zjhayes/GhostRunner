using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] MovementManager movementManager;

    private Animator animator;

    private static readonly int DirX = Animator.StringToHash(AnimatorProperty.DIRECTION_X);
    private static readonly int DirY = Animator.StringToHash(AnimatorProperty.DIRECTION_Y);
    private static readonly int Moving = Animator.StringToHash(AnimatorProperty.MOVING);

        private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void UpdateDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        Vector2 dir = Conversion.QuantizeToCardinal(direction);

        animator.SetFloat(DirX, dir.x);
        animator.SetFloat(DirY, dir.y);
    }

    private void UpdateMoving(bool isMoving)
    {
        animator.SetBool(Moving, isMoving);
    }

    private void OnEnable()
    {
        if (movementManager == null) return;

        movementManager.OnDirectionChanged += UpdateDirection;
        movementManager.OnMovingChanged += UpdateMoving;

        UpdateMoving(movementManager.IsMoving);
        UpdateDirection(movementManager.Direction);
    }

    private void OnDisable()
    {
        if (movementManager == null) return;

        movementManager.OnDirectionChanged -= UpdateDirection;
        movementManager.OnMovingChanged -= UpdateMoving;
    }
}
