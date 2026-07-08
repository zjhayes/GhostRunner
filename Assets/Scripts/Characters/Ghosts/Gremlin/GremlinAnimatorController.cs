using UnityEngine;

public class GremlinAnimatorController : MonoBehaviour
{
    [SerializeField] private FlipbookAnimator animator;
    [SerializeField] private Gremlin gremlin;

    private AnimationGroup currentAnimationGroup;

    private void Awake()
    {
        if (!gremlin)
            gremlin = GetComponentInParent<Gremlin>();

        if (!animator)
            animator = GetComponent<FlipbookAnimator>();
    }

    private void OnEnable()
    {
        if (gremlin == null)
            return;

        gremlin.Movement.OnDirectionChanged += HandleDirectionChanged;
        gremlin.OnAnimationGroupChanged += HandleAnimationGroupChanged;

        HandleAnimationGroupChanged(gremlin.CurrentAnimationGroup);
        HandleDirectionChanged(gremlin.Movement.Direction);
    }

    private void OnDisable()
    {
        if (gremlin == null)
            return;

        gremlin.Movement.OnDirectionChanged -= HandleDirectionChanged;
        gremlin.OnAnimationGroupChanged -= HandleAnimationGroupChanged;
    }

    private void HandleAnimationGroupChanged(AnimationGroup group)
    {
        currentAnimationGroup = group;
        RefreshAnimation();
    }

    private void HandleDirectionChanged(Cardinal direction)
    {
        RefreshAnimation();
    }

    private void RefreshAnimation()
    {
        if (animator == null || currentAnimationGroup == null || gremlin == null)
            return;

        animator.Animation = currentAnimationGroup.GetAnimation(gremlin.Movement.Direction);
    }
}