using System;
using UnityEngine;

public abstract class CharacterManager : GameBehaviour
{
    [SerializeField] protected MovementManager movement;
    public MovementManager Movement { get { return movement; } }
    public event Action<Collision2D> OnCollisionEntered;
    public event Action<Collider2D> OnTriggerEnter;

    protected virtual void Start()
    {
        Movement.EdgeResolver = HandleResolveEdge;
    }

    public virtual void ResetState()
    {
        Movement.ResetState();
        Active();
    }

    public virtual void Active(bool enabled = true)
    {
        gameObject.SetActive(enabled);
    }

    protected virtual EdgeTraversalResult HandleResolveEdge(Node node, Cardinal direction)
    {
        return node.ResolveEdge(this, direction);
    }

    protected virtual void HandleCollision(Collision2D other)
    {
        OnCollisionEntered?.Invoke(other);
    }

    protected virtual void HandleTrigger(Collider2D other)
    {
        OnTriggerEnter?.Invoke(other);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrigger(other);
    }
}
