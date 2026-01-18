using System;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class MovementManager : MonoBehaviour
{
    [SerializeField] float speed = 8.0f;
    [SerializeField] Vector2 initialDirection;
    [SerializeField] LayerMask obstacleLayer;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector3 StartingPosition {  get; private set; }
    public Vector2 NextDirection { get; private set; }
    public float SpeedMultiplier = 1.0f;

    private float castSize = 0.75f;
    private float castDistance = 1.5f;

    public event Action<Vector2> DirectionChanged;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D> ();
        StartingPosition = transform.position;
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
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            direction.Normalize();

            if (direction != Direction)
            {
                DirectionChanged?.Invoke(direction);
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
        SpeedMultiplier = 1.0f;
        Direction = initialDirection;
        NextDirection = Vector2.zero;
        transform.position = StartingPosition;
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        DirectionChanged?.Invoke(Direction);
    }
}
