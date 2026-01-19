using System;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class MovementManager : MonoBehaviour
{
    [SerializeField] float speed = 8.0f;
    [SerializeField] Vector2 initialDirection;
    [SerializeField] LayerMask obstacleLayer;

    private float castSize = 0.75f;
    private float castDistance = 1.5f;
    private bool isMoving;
    private Vector2 lastPosition;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector3 StartingPosition {  get; private set; }
    public float SpeedMultiplier { get; set; } = 1.0f;
    public Vector2 NextDirection { get; private set; }
    public bool IsMoving
    {
        get => isMoving;
        private set
        {
            if (isMoving == value) return;
            isMoving = value;
            OnMovingChanged?.Invoke(isMoving);
        }
    }

    public event Action<bool> OnMovingChanged;
    
    public event Action<Vector2> OnDirectionChanged;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D> ();
        StartingPosition = transform.position;
        lastPosition = Rigidbody.position;
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        if (NextDirection != Vector2.zero)
        {
            SetDirection (NextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = Rigidbody.position;
        Vector2 translation = Direction * speed * SpeedMultiplier * Time.fixedDeltaTime;
        Rigidbody.MovePosition(position + translation);
        
        Vector2 delta = position - lastPosition;
        IsMoving = delta.sqrMagnitude > Numeric.MINIMUM_MAGNITUDE;
        lastPosition = position;
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            direction.Normalize();

            if (direction != Direction)
            {
                OnDirectionChanged?.Invoke(direction);
                Direction = direction;
            }

            NextDirection = Vector2.zero;
        }
        else
        {
            NextDirection = direction;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * castSize, 0.0f, direction, castDistance, obstacleLayer);
        return hit.collider != null;
    }

    public void ResetState()
    {
        SpeedMultiplier = Numeric.WHOLE;
        Direction = initialDirection;
        NextDirection = Vector2.zero;
        Rigidbody.position = StartingPosition;
        lastPosition = Rigidbody.position;
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        OnDirectionChanged?.Invoke(Direction);
    }
}
