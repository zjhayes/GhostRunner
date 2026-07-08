using System;
using UnityEngine;

[RequireComponent(typeof(GremlinLanternFear))]
public class Gremlin : Ghost
{
    public GremlinContext GremlinContext { get; private set; }

    public bool IsFrightened => GremlinContext != null && GremlinContext.IsFrightened;
    public bool IsHiding => GremlinContext != null && GremlinContext.IsHiding;

    public AnimationGroup CurrentAnimationGroup =>
        GremlinContext != null ? GremlinContext.CurrentAnimationGroup : null;

    public event Action OnFrightened;
    public event Action OnCalmed;
    public event Action OnHiding;
    public event Action<AnimationGroup> OnAnimationGroupChanged;

    protected override void Awake()
    {
        base.Awake();

        GremlinContext = Context as GremlinContext;

        if (GremlinContext == null)
        {
            Debug.LogError($"{name} needs a GremlinContext assigned to Ghost.Context.", this);
            return;
        }

        GremlinContext.OnFrightened += HandleFrightened;
        GremlinContext.OnCalmed += HandleCalmed;
        GremlinContext.OnHiding += HandleHiding;
        GremlinContext.OnAnimationGroupChanged += HandleAnimationGroupChanged;
    }

    private void OnDestroy()
    {
        if (GremlinContext == null)
            return;

        GremlinContext.OnFrightened -= HandleFrightened;
        GremlinContext.OnCalmed -= HandleCalmed;
        GremlinContext.OnHiding -= HandleHiding;
        GremlinContext.OnAnimationGroupChanged -= HandleAnimationGroupChanged;
    }

    public void Frighten()
    {
        Frighten(0f);
    }

    public void Frighten(float duration)
    {
        if (GremlinContext == null)
            return;

        bool wasFrightened = GremlinContext.IsFrightened;

        GremlinContext.Frighten(duration);

        if (!wasFrightened && GremlinContext.IsFrightened)
            GremlinContext.Frightened.FleeFrom(Target);

        if (!GremlinContext.IsHiding)
            movement.SpeedMultiplier = 3f;
    }

    public void Calm()
    {
        if (GremlinContext == null)
            return;

        GremlinContext.Calm();
    }

    private void HandleFrightened()
    {
        OnFrightened?.Invoke();
    }

    private void HandleCalmed()
    {
        movement.SpeedMultiplier = 1f;
        OnCalmed?.Invoke();
    }

    private void HandleHiding()
    {
        OnHiding?.Invoke();
    }

    private void HandleAnimationGroupChanged(AnimationGroup group)
    {
        OnAnimationGroupChanged?.Invoke(group);
    }
}
