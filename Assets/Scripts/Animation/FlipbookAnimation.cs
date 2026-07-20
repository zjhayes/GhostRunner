using System;
using UnityEngine;
using UnityEngine.Events;

public class FlipbookAnimation : MonoBehaviour
{
    [Serializable]
    private class FrameEvent
    {
        [Tooltip("Zero-based frame within this animation.")]
        [Min(0)]
        [SerializeField] private int frame;

        [SerializeField] private UnityEvent callback;

        public int Frame => frame;

        public void Invoke()
        {
            callback?.Invoke();
        }
    }

    [Header("Textures")]
    public Texture2D baseSpritesheet;
    public Texture2D normalSpritesheet;
    public Texture2D emissionsSpritesheet;

    [Header("Sheet")]
    public int columns = 6;
    public int rows = 4;

    [Header("Animation")]
    public int startFrame = 0;
    public int frameCount = 24;
    public float framesPerSecond = 12f;

    [Header("Events")]
    [SerializeField] private FrameEvent[] frameEvents = Array.Empty<FrameEvent>();

    internal void InvokeFrameEvents(int localFrame)
    {
        if (frameEvents == null)
            return;

        foreach (FrameEvent frameEvent in frameEvents)
        {
            if (frameEvent != null && frameEvent.Frame == localFrame)
                frameEvent.Invoke();
        }
    }
}
