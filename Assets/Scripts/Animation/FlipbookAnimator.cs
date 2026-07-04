using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlipbookAnimator : MonoBehaviour
{
    private FlipbookAnimation flipbookAnimation;
    private Renderer rend;
    private MaterialPropertyBlock block;
    private float timeOffset;

    private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");
    private static readonly int NormalMapID = Shader.PropertyToID("_NormalMap");
    private static readonly int EmissionMapID = Shader.PropertyToID("_EmissionMap");

    private static readonly int FrameIndexID = Shader.PropertyToID("_FrameIndex");
    private static readonly int ColumnsID = Shader.PropertyToID("_Columns");
    private static readonly int RowsID = Shader.PropertyToID("_Rows");

    public FlipbookAnimation Animation
    {
        set
        {
            flipbookAnimation = value;

            if (rend == null)
            {
                rend = GetComponent<Renderer>();
            }

            if (block == null)
            {
                block = new MaterialPropertyBlock();
            }

            ApplyStaticProperties();
        }
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (flipbookAnimation == null) { return; }
        if (flipbookAnimation.frameCount <= 0) { return; }
        if (flipbookAnimation.framesPerSecond <= 0f) { return; }

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
        if (flipbookAnimation == null) { return; }

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

        if (flipbookAnimation.emissionsSpritesheet != null)
        {
            block.SetTexture(EmissionMapID, flipbookAnimation.emissionsSpritesheet);
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