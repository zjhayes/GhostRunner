using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(
    fileName = "FootstepSurface",
    menuName = "Audio/Footstep Surface")]
public class FootstepSurface : ScriptableObject
{
    [Header("Clips")]
    public AudioClip[] walkClips;
    public AudioClip[] runClips;

    [Header("Volume")]
    [Range(0f, 1f)]
    [FormerlySerializedAs("volume")]
    public float walkVolume = 0.05f;

    [Range(0f, 1f)]
    public float runVolume = 0.1f;

    [Header("Variation")]
    [Min(0f)]
    public float volumeVariation = 0.08f;

    public Vector2 pitchRange = new Vector2(0.94f, 1.06f);
}
