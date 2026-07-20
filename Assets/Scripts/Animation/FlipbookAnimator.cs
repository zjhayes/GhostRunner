using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlipbookAnimator : MonoBehaviour
{
    private FlipbookAnimation flipbookAnimation;
    private Renderer rend;
    private MaterialPropertyBlock block;
    private float timeOffset;
    private bool loop = true;
    private bool completionRaised;
    private int previousElapsedFrame = -1;

    public event Action AnimationCompleted;

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
            loop = true;
            previousElapsedFrame = -1;

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

        float elapsedTime = Mathf.Max(0f, Time.time + timeOffset);
        int elapsedFrames = Mathf.FloorToInt(elapsedTime * flipbookAnimation.framesPerSecond);
        int localFrame = loop
            ? elapsedFrames % flipbookAnimation.frameCount
            : Mathf.Min(elapsedFrames, flipbookAnimation.frameCount - 1);

        int frame = flipbookAnimation.startFrame + localFrame;

        rend.GetPropertyBlock(block);
        block.SetFloat(FrameIndexID, frame);
        rend.SetPropertyBlock(block);

        InvokeFrameEvents(elapsedFrames);

        if (!loop && !completionRaised && elapsedFrames >= flipbookAnimation.frameCount - 1)
        {
            completionRaised = true;
            AnimationCompleted?.Invoke();
        }
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

    private void InvokeFrameEvents(int elapsedFrames)
    {
        int firstElapsedFrame = previousElapsedFrame < 0
            ? elapsedFrames
            : previousElapsedFrame + 1;

        // A restart or a long suspension should not replay stale events in a burst.
        if (elapsedFrames < previousElapsedFrame ||
            elapsedFrames - firstElapsedFrame >= flipbookAnimation.frameCount)
        {
            firstElapsedFrame = elapsedFrames;
        }

        for (int elapsedFrame = firstElapsedFrame;
             elapsedFrame <= elapsedFrames;
             elapsedFrame++)
        {
            if (!loop && elapsedFrame >= flipbookAnimation.frameCount)
                break;

            int localFrame = loop
                ? elapsedFrame % flipbookAnimation.frameCount
                : elapsedFrame;

            flipbookAnimation.InvokeFrameEvents(localFrame);
        }

        previousElapsedFrame = elapsedFrames;
    }

    public void Play(FlipbookAnimation newAnimation)
    {
        Play(newAnimation, true);
    }

    public void Play(FlipbookAnimation newAnimation, bool loopAnimation)
    {
        if (newAnimation == null)
        {
            Debug.LogWarning($"{nameof(FlipbookAnimator)} tried to play a null animation.", this);
            return;
        }

        flipbookAnimation = newAnimation;
        loop = loopAnimation;
        timeOffset = -Time.time;
        completionRaised = false;
        previousElapsedFrame = -1;
        ApplyStaticProperties();
    }

    public void Restart()
    {
        timeOffset = -Time.time;
        completionRaised = false;
        previousElapsedFrame = -1;
    }
}
