using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostEyes : MonoBehaviour
{
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;

    private SpriteRenderer spriteRenderer;
    private MovementManager movement;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponentInParent<MovementManager>();
    }

    private void OnEnable()
    {
        if (movement == null) return;

        movement.OnDirectionChanged += HandleDirectionChanged;
        HandleDirectionChanged(movement.Direction); // initialize immediately
    }

    private void OnDisable()
    {
        if (movement == null) return;
        movement.OnDirectionChanged -= HandleDirectionChanged;
    }

    private void HandleDirectionChanged(Cardinal dir)
    {
        spriteRenderer.sprite = dir switch
        {
            Cardinal.North => up,
            Cardinal.South => down,
            Cardinal.West  => left,
            Cardinal.East  => right,
            _ => spriteRenderer.sprite
        };
    }
}

