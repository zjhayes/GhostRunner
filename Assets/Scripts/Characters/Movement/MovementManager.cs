using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementManager : MonoBehaviour
{
    [SerializeField] float speed = 8.0f;
    [SerializeField] Vector2 initialDirection = Vector2.right;

    [Header("Node Centering")]
    [SerializeField] float centerEpsilon = 0.01f;

    private bool isMoving;
    private Vector2 lastPosition;
    private Node currentNode;
    private Node targetNode;
    private Node lastEnteredNode;

    private int nodesLayer;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector3 StartingPosition { get; private set; }
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
        Rigidbody = GetComponent<Rigidbody2D>();
        StartingPosition = Rigidbody.position;
        lastPosition = Rigidbody.position;

        nodesLayer = LayerMask.NameToLayer(Layer.NODES);

        if (Rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    private void Start()
    {
        ResetState();
    }

    private void FixedUpdate()
    {
        float stepRemaining = speed * SpeedMultiplier * Time.fixedDeltaTime;
        Vector2 newPos = Rigidbody.position;

        while (stepRemaining > Numeric.MILLIONTH)
        {
            if (targetNode != null)
            {
                Vector2 center = targetNode.transform.position;
                Vector2 toCenter = center - newPos;
                float dist = toCenter.magnitude;

                if (dist <= centerEpsilon)
                {
                    // Snap to exact center.
                    newPos = center;

                    // We are now officially "at" this node.
                    currentNode = targetNode;

                    // Decide direction at node center.
                    Vector2 chosen = ChooseDirectionAtNode(targetNode);

                    // Done centering.
                    targetNode = null;

                    if (chosen == Vector2.zero)
                    {
                        ApplyDirection(Vector2.zero);
                        stepRemaining = 0f;
                        break;
                    }

                    ApplyDirection(chosen);
                    // Continue consuming remaining step outward.
                    continue;
                }

                float moveDist = Mathf.Min(stepRemaining, dist);
                newPos += (toCenter / dist) * moveDist;
                stepRemaining -= moveDist;
                continue;
            }

            // Free movement between nodes (straight line).
            if (Direction == Vector2.zero)
                break;

            newPos += Direction * stepRemaining;
            stepRemaining = 0f;
        }

        Rigidbody.MovePosition(newPos);
        UpdateMovingState(newPos);
    }

    private void UpdateMovingState(Vector2 newPos)
    {
        Vector2 delta = newPos - lastPosition;
        IsMoving = delta.sqrMagnitude > 0.000001f;
        lastPosition = newPos;
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (direction == Vector2.zero) return;

        direction = Conversion.QuantizeToCardinal(direction);

        // Reversal: allow ANYTIME while moving.
        if (Direction != Vector2.zero && direction == -Direction)
        {
            ApplyDirection(direction);
            NextDirection = Vector2.zero;

            // If we're still inside a node trigger, re-center to THAT node once.
            if (currentNode != null)
            {
                targetNode = currentNode;
            }
            else
            {
                // Between nodes: do NOT force centering; just reverse cleanly.
                targetNode = null;
            }

            return;
        }

        // If we're stopped on a node, allow immediate start if valid.
        if (Direction == Vector2.zero && currentNode != null)
        {
            if (forced || currentNode.AvailableDirections.Contains(direction))
            {
                ApplyDirection(direction);
                NextDirection = Vector2.zero;
                return;
            }
        }

        // Otherwise buffer until the next node center.
        NextDirection = direction;
    }

    private void ApplyDirection(Vector2 dir)
    {
        dir = Conversion.QuantizeToCardinal(dir);

        if (dir == Direction) return;

        Direction = dir;
        OnDirectionChanged?.Invoke(Direction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != nodesLayer) return;

        Node node = other.GetComponent<Node>() ?? other.GetComponentInParent<Node>();
        if (node == null) return;

        currentNode = node;

        // Prevent repeatedly re-locking the same node while we remain overlapping it.
        if (lastEnteredNode == node)
            return;

        // Only decide whether to lock if we are NOT already centering.
        if (targetNode == null && Direction != Vector2.zero)
        {
            Vector2 toNode = (Vector2)node.transform.position - Rigidbody.position;

            // If the node is behind us relative to our current movement, don't lock onto it.
            if (Vector2.Dot(Direction, toNode) <= 0f)
                return;
        }

        lastEnteredNode = node;

        // Lock onto this node so we pass through the center.
        targetNode = node;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != nodesLayer) return;

        Node node = other.GetComponent<Node>() ?? other.GetComponentInParent<Node>();
        if (node == null) return;

        if (currentNode == node)
            currentNode = null;

        if (lastEnteredNode == node)
            lastEnteredNode = null;
    }

    private Vector2 ChooseDirectionAtNode(Node node)
    {
        if (NextDirection != Vector2.zero && node.AvailableDirections.Contains(NextDirection))
        {
            Vector2 chosen = NextDirection;
            NextDirection = Vector2.zero;
            return chosen;
        }

        if (Direction != Vector2.zero && node.AvailableDirections.Contains(Direction))
        {
            return Direction;
        }

        return Vector2.zero;
    }

    public void ResetState()
    {
        SpeedMultiplier = 1f;
        Direction = Conversion.QuantizeToCardinal(initialDirection);
        NextDirection = Vector2.zero;

        Rigidbody.position = StartingPosition;
        lastPosition = Rigidbody.position;

        currentNode = null;
        targetNode = null;
        lastEnteredNode = null;

        OnDirectionChanged?.Invoke(Direction);
    }
}
