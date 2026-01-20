using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementManager : MonoBehaviour
{
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private Vector2 initialDirection = Vector2.right;

    [Header("Node Centering")]
    [SerializeField] private float centerEpsilon = 0.01f;

    private bool isMoving;
    private Vector2 lastPosition;

    private Node currentNode;     // node trigger we are currently inside (may be null between nodes)
    private Node targetNode;      // node we are currently centering to (null if free-moving)
    private Node lastEnteredNode; // used to prevent re-locking same node while overlapping

    private int nodesLayer;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector3 StartingPosition { get; private set; }
    public float SpeedMultiplier { get; set; } = 1.0f;

    public Cardinal Direction { get; private set; }
    public Cardinal? NextDirection { get; private set; }

    public bool IsStopped => DirectionVector == Vector2.zero;

    // Convenience for physics
    public Vector2 DirectionVector { get; private set; } = Vector2.right;

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

    // Prefer subscribing to this in other systems (lantern, shadow, anim, etc.)
    public event Action<Cardinal> OnDirectionChanged;

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
            // Centering phase: move to exact node center.
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
                    Cardinal? chosen = ChooseDirectionAtNode(targetNode);

                    // Done centering.
                    targetNode = null;

                    if (!chosen.HasValue)
                    {
                        Stop();
                        stepRemaining = 0f;
                        break;
                    }

                    ApplyDirection(chosen.Value);
                    // Continue consuming remaining step outward in the same FixedUpdate.
                    continue;
                }

                float moveDist = Mathf.Min(stepRemaining, dist);
                newPos += (toCenter / dist) * moveDist;
                stepRemaining -= moveDist;
                continue;
            }

            // Free movement between nodes.
            if (DirectionVector == Vector2.zero)
                break;

            newPos += DirectionVector * stepRemaining;
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

    public void SetDirection(Vector2 inputDirection, bool forced = false)
    {
        if (inputDirection == Vector2.zero) return;

        Cardinal requested = CardinalUtil.FromVector(inputDirection, Direction);

        // Reversal: allow ANYTIME while moving.
        if (DirectionVector != Vector2.zero && CardinalUtil.IsOpposite(Direction, requested))
        {
            ApplyDirection(requested);
            NextDirection = null;

            // If we're still inside a node trigger, re-center to THAT node once.
            if (currentNode != null)
                targetNode = currentNode;
            else
                targetNode = null; // between nodes: reverse cleanly without forcing centering

            return;
        }

        // If we're stopped on a node, allow immediate start if valid.
        if (DirectionVector == Vector2.zero && currentNode != null)
        {
            if (forced || currentNode.AvailableDirections.Contains(requested))
            {
                ApplyDirection(requested);
                NextDirection = null;
                return;
            }
        }

        // Otherwise buffer until the next node center.
        NextDirection = requested;
    }

    public void SetDirection(Cardinal requested, bool forced = false)
    {
        SetDirection(CardinalUtil.ToVector(requested), forced);
    }

    private void Stop()
    {
        // Keep Direction as the last facing direction, but stop movement by zeroing vector.
        DirectionVector = Vector2.zero;
        NextDirection = null;
    }

    private void ApplyDirection(Cardinal dir)
    {
        // If we're currently stopped, treat this as start moving in that direction.
        // If we are moving, it might be a turn/reversal.
        Direction = dir;
        DirectionVector = CardinalUtil.ToVector(dir);

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
        if (targetNode == null && DirectionVector != Vector2.zero)
        {
            Vector2 toNode = (Vector2)node.transform.position - Rigidbody.position;

            // If the node is behind us relative to our current movement, don't lock onto it.
            if (Vector2.Dot(DirectionVector, toNode) <= 0f)
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

    private Cardinal? ChooseDirectionAtNode(Node node)
    {
        if (NextDirection.HasValue && node.AvailableDirections.Contains(NextDirection.Value))
        {
            Cardinal chosen = NextDirection.Value;
            NextDirection = null;
            return chosen;
        }

        // Continue forward if possible.
        if (DirectionVector != Vector2.zero && node.AvailableDirections.Contains(Direction))
        {
            return Direction;
        }

        return null;
    }

    public void ResetState()
    {
        SpeedMultiplier = 1f;

        Direction = CardinalUtil.FromVector(initialDirection, Cardinal.East);
        DirectionVector = CardinalUtil.ToVector(Direction);

        NextDirection = null;

        Rigidbody.position = StartingPosition;
        lastPosition = Rigidbody.position;

        currentNode = null;
        targetNode = null;
        lastEnteredNode = null;

        OnDirectionChanged?.Invoke(Direction);
    }
}
