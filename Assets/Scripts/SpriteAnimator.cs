using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float animationTime = 0.25f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool bounce = false;

    public int Index { get; private set; }
    public SpriteRenderer Renderer { get; private set; }

    private int direction = 1; // 1 = forward, -1 = backward

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (sprites == null || sprites.Length == 0) return;

        Index = 0;
        Renderer.sprite = sprites[Index];

        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }

    private void Advance()
    {
        if (!Renderer.enabled) return;
        if (sprites == null || sprites.Length == 0) return;

        Index += direction;

        int last = sprites.Length - 1;

        if (Index > last)
        {
            if (bounce && sprites.Length > 1)
            {
                direction = -1;
                Index = last - 1; // don’t repeat the last frame
            }
            else if (loop)
            {
                Index = 0;
            }
            else
            {
                Index = last;
                CancelInvoke(nameof(Advance));
            }
        }
        else if (Index < 0)
        {
            if (bounce && sprites.Length > 1)
            {
                direction = 1;
                Index = 1; // don’t repeat the first frame
            }
            else if (loop)
            {
                Index = last;
            }
            else
            {
                Index = 0;
                CancelInvoke(nameof(Advance));
            }
        }

        Renderer.sprite = sprites[Index];
    }

    public void Restart()
    {
        if (sprites == null || sprites.Length == 0) return;

        direction = 1;
        Index = 0;
        Renderer.sprite = sprites[Index];

        CancelInvoke(nameof(Advance));
        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }
}
