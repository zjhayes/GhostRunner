using System;
using UnityEngine;

[RequireComponent(typeof(GremlinFrightened))]
public class GremlinContext : GhostContext
{
    public enum GremlinState
    {
        Walking,
        Running,
        Frightened,
        Hiding,
        Idle
    }

    [Header("Animation Groups")]
    [SerializeField] private AnimationGroup walkingAnimations;
    [SerializeField] private AnimationGroup runningAnimations;
    [SerializeField] private AnimationGroup escapingAnimations;
    [SerializeField] private AnimationGroup hidingAnimations;
    [SerializeField] private AnimationGroup idleAnimations;

    private GremlinFrightened frightened;
    private GremlinState state = GremlinState.Walking;

    public GremlinFrightened Frightened => frightened;

    public AnimationGroup WalkingAnimations => walkingAnimations;
    public AnimationGroup RunningAnimations => runningAnimations;
    public AnimationGroup EscapingAnimations => escapingAnimations;
    public AnimationGroup HidingAnimations => hidingAnimations;

    public AnimationGroup CurrentAnimationGroup => state switch
    {
        GremlinState.Running => runningAnimations != null ? runningAnimations : walkingAnimations,
        GremlinState.Frightened => escapingAnimations,
        GremlinState.Hiding => hidingAnimations != null ? hidingAnimations : escapingAnimations,
        GremlinState.Idle => idleAnimations != null ? idleAnimations : walkingAnimations,
        _ => walkingAnimations
    };

    public GremlinState State => state;
    public bool IsFrightened => state == GremlinState.Frightened;
    public bool IsHiding => state == GremlinState.Hiding;
    public bool IsRunning => state == GremlinState.Running;

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

    public override void EnterScatter()
    {
        CancelInvoke(nameof(Calm));

        if (frightened != null)
            frightened.Disable();

        state = GremlinState.Walking;
        base.EnterScatter();

        OnCalmed?.Invoke();
        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    public void Frighten()
    {
        Frighten(0f);
    }

    public void Frighten(float duration)
    {
        if (IsIdle)
            return;

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

    public void SetRunning(bool running)
    {
        if (IsIdle || IsFrightened || IsHiding)
            return;

        GremlinState nextState = running ? GremlinState.Running : GremlinState.Walking;
        if (state == nextState)
            return;

        state = nextState;
        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    public void Calm()
    {
        if (IsIdle)
            return;

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

    public override void EnterIdle()
    {
        if (IsIdle)
            return;

        CancelInvoke(nameof(Calm));

        if (frightened != null)
            frightened.Disable();

        state = GremlinState.Idle;
        base.EnterIdle();
        OnAnimationGroupChanged?.Invoke(CurrentAnimationGroup);
    }

    private void ResetFearTimer(float duration)
    {
        CancelInvoke(nameof(Calm));

        if (duration > 0f)
            Invoke(nameof(Calm), duration);
    }
}
