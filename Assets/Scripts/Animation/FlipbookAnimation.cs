using UnityEngine;

public class FlipbookAnimation : MonoBehaviour
{
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
}