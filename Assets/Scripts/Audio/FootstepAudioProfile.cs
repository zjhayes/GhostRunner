using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FootstepAudioProfile",
    menuName = "Audio/Footstep Audio Profile")]
public class FootstepAudioProfile : ScriptableObject
{
    [Serializable]
    private class SurfaceSounds
    {
        [SerializeField] internal FootstepSurfaceType surfaceType;
        [SerializeField] internal FootstepSurface sounds;
    }

    [Tooltip("Used when detection misses or this character has no sounds for the detected surface.")]
    [SerializeField] private FootstepSurface fallbackSounds;

    [SerializeField] private SurfaceSounds[] surfaceSounds = Array.Empty<SurfaceSounds>();

    public FootstepSurface Resolve(FootstepSurfaceType? surfaceType)
    {
        if (surfaceType.HasValue && surfaceSounds != null)
        {
            foreach (SurfaceSounds mapping in surfaceSounds)
            {
                if (mapping != null &&
                    mapping.surfaceType == surfaceType.Value &&
                    mapping.sounds != null)
                {
                    return mapping.sounds;
                }
            }
        }

        return fallbackSounds;
    }
}
