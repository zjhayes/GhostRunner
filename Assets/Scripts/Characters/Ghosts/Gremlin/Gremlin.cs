using System;
using UnityEngine;

[RequireComponent(typeof(GremlinLanternFear))]
public class Gremlin : Ghost
{
    [Header("Player Proximity Chase")]
    [SerializeField, Min(0f)] private float chaseDistance = 6f;
    [SerializeField, Min(0f)] private float scatterDistance = 9f;

    public GremlinContext GremlinContext { get; private set; }

    private bool isPlayerClose;

    public bool IsFrightened => GremlinContext != null && GremlinContext.IsFrightened;
    public bool IsHiding => GremlinContext != null && GremlinContext.IsHiding;
    public bool IsRunning => GremlinContext != null && GremlinContext.IsRunning;

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

    private void Update()
    {
        UpdatePlayerProximity();
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

        if (GremlinContext.IsFrightened || GremlinContext.IsHiding)
            GremlinContext.Calm();
    }

    public void Chase()
    {
        if (GremlinContext == null)
            return;

        if (IsFrightened || IsHiding)
            GremlinContext.Calm();

        if (GremlinContext.Scatter != null && GremlinContext.Scatter.enabled)
            GremlinContext.Scatter.Disable();

        if (GremlinContext.Chase != null)
            GremlinContext.Chase.Enable();
    }

    public void Scatter()
    {
        if (GremlinContext == null || IsFrightened || IsHiding || IsRunning)
            return;

        if (GremlinContext.Chase != null && GremlinContext.Chase.enabled)
        {
            GremlinContext.Chase.Disable();
            return;
        }

        if (GremlinContext.Scatter != null && !GremlinContext.Scatter.enabled)
            GremlinContext.Scatter.Enable();
    }

    public void Run()
    {
        if (GremlinContext == null || IsFrightened || IsHiding)
            return;

        if (!GremlinContext.IsRunning)
            GremlinContext.SetRunning(true);

        movement.SpeedMultiplier = 2f;
    }

    public void StopRunning()
    {
        if (GremlinContext == null)
            return;

        if (!GremlinContext.IsRunning)
            return;

        GremlinContext.SetRunning(false);
        movement.SpeedMultiplier = 1f;
    }

    public bool IsFacing(Transform target)
    {
        if (target == null)
            return false;

        Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;
        Cardinal targetDirection = CardinalUtil.FromVector(toTarget, movement.Direction);
        return movement.Direction == targetDirection;
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

    private void UpdatePlayerProximity()
    {
        if (Target == null || GremlinContext == null || GremlinContext.IsIdle)
            return;

        float distanceSquared = ((Vector2)Target.position - (Vector2)transform.position).sqrMagnitude;

        if (!isPlayerClose && distanceSquared <= chaseDistance * chaseDistance)
            isPlayerClose = true;
        else if (isPlayerClose && distanceSquared >= scatterDistance * scatterDistance)
        {
            isPlayerClose = false;
            StopRunning();
            Scatter();
        }

        if (isPlayerClose && IsFacing(Target) && !IsFrightened && !IsHiding)
        {
            Run();
            Chase();
        }
    }

    private void OnValidate()
    {
        scatterDistance = Mathf.Max(scatterDistance, chaseDistance);
    }
}
