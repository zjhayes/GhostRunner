using System;
using UnityEngine;

[RequireComponent(typeof(MovementManager))]
public abstract class CharacterManager : MonoBehaviour
{
    public MovementManager Movement { get; protected set; }
    public event Action<Collision2D> OnCollisionEntered;
    public event Action<Collider2D> OnTriggerEnter;

    protected virtual void Awake()
    {
        Movement = GetComponent<MovementManager>();
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
