using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class GhostEyes : MonoBehaviour
{
    [SerializeField] Sprite up;
    [SerializeField] Sprite down;
    [SerializeField] Sprite left;
    [SerializeField] Sprite right;

    private SpriteRenderer spriteRenderer;
    private MovementManager movement;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponentInParent<MovementManager>();
    }

    private void Update()
    {
        if (movement.Direction == Vector2.up)
        {
            spriteRenderer.sprite = up;
        }
        else if (movement.Direction == Vector2.down)
        {
            spriteRenderer.sprite = down;
        }
        else if (movement.Direction == Vector2.left)
        {
            spriteRenderer.sprite = left;
        }
        else if (movement.Direction == Vector2.right)
        {
            spriteRenderer.sprite = right;
        }
    }
}
