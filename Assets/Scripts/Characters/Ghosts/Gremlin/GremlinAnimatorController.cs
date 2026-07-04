using UnityEngine;

public class GremlinAnimatorController : MonoBehaviour
{
    [SerializeField] FlipbookAnimator animator;
    [SerializeField] AnimationGroup walkingAnimations;
    [SerializeField] Gremlin gremlin;

    private void OnEnable()
    {
        gremlin.Movement.OnDirectionChanged += HandleDirectionChanged;
    }

    private void OnDisable()
    {
        gremlin.Movement.OnDirectionChanged -= HandleDirectionChanged;
    }

    private void HandleDirectionChanged(Cardinal direction)
    {
        animator.Animation = walkingAnimations.GetAnimation(direction);
    }


}
