using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlipbookAnimator : MonoBehaviour
{
    [SerializeField] private FlipbookAnimation flipbookAnimation;

    private Renderer rend;
    private MaterialPropertyBlock block;
    private float timeOffset;

    private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");
    private static readonly int NormalMapID = Shader.PropertyToID("_NormalMap");
    private static readonly int FrameIndexID = Shader.PropertyToID("_FrameIndex");
    private static readonly int ColumnsID = Shader.PropertyToID("_Columns");
    private static readonly int RowsID = Shader.PropertyToID("_Rows");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        if (flipbookAnimation == null)
        {
            flipbookAnimation = GetComponent<FlipbookAnimation>();
        }

        if (flipbookAnimation == null)
        {
            Debug.LogError($"{nameof(FlipbookAnimator)} on {name} needs a {nameof(FlipbookAnimation)} assigned.", this);
            enabled = false;
            return;
        }

        if (flipbookAnimation.randomizeStart)
        {
            timeOffset = Random.value * flipbookAnimation.frameCount / flipbookAnimation.framesPerSecond;
        }

        ApplyStaticProperties();
    }

    private void LateUpdate()
    {
        int localFrame = Mathf.FloorToInt(
            (Time.time + timeOffset) * flipbookAnimation.framesPerSecond
        ) % flipbookAnimation.frameCount;

        int frame = flipbookAnimation.startFrame + localFrame;

        rend.GetPropertyBlock(block);
        block.SetFloat(FrameIndexID, frame);
        rend.SetPropertyBlock(block);
    }

    private void ApplyStaticProperties()
    {
        rend.GetPropertyBlock(block);

        block.SetFloat(ColumnsID, flipbookAnimation.columns);
        block.SetFloat(RowsID, flipbookAnimation.rows);

        if (flipbookAnimation.baseSpritesheet != null)
        {
            block.SetTexture(BaseMapID, flipbookAnimation.baseSpritesheet);
        }

        if (flipbookAnimation.normalSpritesheet != null)
        {
            block.SetTexture(NormalMapID, flipbookAnimation.normalSpritesheet);
        }

        rend.SetPropertyBlock(block);
    }

    public void Play(FlipbookAnimation newAnimation)
    {
        if (newAnimation == null)
        {
            Debug.LogWarning($"{nameof(FlipbookAnimator)} tried to play a null animation.", this);
            return;
        }

        flipbookAnimation = newAnimation;
        timeOffset = 0f;
        ApplyStaticProperties();
    }

    public void Restart()
    {
        timeOffset = -Time.time;
    }
}