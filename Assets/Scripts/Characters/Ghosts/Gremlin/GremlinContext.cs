using System;
using UnityEngine;

[RequireComponent(typeof(GremlinFrightened))]
public class GremlinContext : GhostContext
{
    public enum GremlinState
    {
        Walking,
        Frightened,
        Hiding
    }

    [Header("Animation Groups")]
    [SerializeField] private AnimationGroup walkingAnimations;
    [SerializeField] private AnimationGroup escapingAnimations;
    [SerializeField] private AnimationGroup hidingAnimations;

    private GremlinFrightened frightened;
    private GremlinState state = GremlinState.Walking;

    public GremlinFrightened Frightened => frightened;

    public AnimationGroup WalkingAnimations => walkingAnimations;
    public AnimationGroup EscapingAnimations => escapingAnimations;
    public AnimationGroup HidingAnimations => hidingAnimations;

    public AnimationGroup CurrentAnimationGroup => state switch
    {
        GremlinState.Frightened => escapingAnimations,
        GremlinState.Hiding => hidingAnimations != null ? hidingAnimations : escapingAnimations,
        _ => walkingAnimations
    };

    public GremlinState State => state;
    public bool IsFrightened => state == GremlinState.Frightened;
    public bool IsHiding => state == GremlinState.Hiding;

    public event Action OnFrightened;
    public event Action OnCalmed;
    public event Action OnHiding;
    public event Action<AnimationGroup> OnAnimationGroupChanged;

    protected override void Awake()
    {
        base.Awake();

        frightened = GetComponent<GremlinFrightened>();
    }

    public override void ResetState()
    {
        CancelInvoke(nameof(Calm));
        base.ResetState();

        state = GremlinState.Walking;

        if (frightened != null)
            frightened.Disable();

        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    public void Frighten()
    {
        Frighten(0f);
    }

    public void Frighten(float duration)
    {
        ResetFearTimer(duration);

        if (IsHiding)
            return;

        if (IsFrightened)
            return;

        DisableNormalBehaviours();

        state = GremlinState.Frightened;

        if (frightened != null)
            frightened.Enable();

        OnFrightened?.Invoke();
        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    public void Hide()
    {
        if (IsHiding)
            return;

        state = GremlinState.Hiding;

        if (frightened != null)
            frightened.Disable();

        if (Ghost != null)
            Ghost.Movement.Stop();

        OnHiding?.Invoke();
        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    public void Calm()
    {
        CancelInvoke(nameof(Calm));

        if (state == GremlinState.Walking)
            return;

        state = GremlinState.Walking;

        if (frightened != null)
            frightened.Disable();

        Scatter.Enable();

        if (Ghost != null && Ghost.Movement.IsStopped)
            Ghost.Movement.SetDirection(CardinalUtil.Opposite(Ghost.Movement.Direction));

        OnCalmed?.Invoke();
        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    private void ResetFearTimer(float duration)
    {
        CancelInvoke(nameof(Calm));

        if (duration > 0f)
            Invoke(nameof(Calm), duration);
    }
}
