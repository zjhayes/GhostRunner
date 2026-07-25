using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RandomAudioCue",
    menuName = "Audio/Random Audio Cue")]
public class RandomAudioCue : ScriptableObject
{
    [SerializeField] private AudioClip[] clips = Array.Empty<AudioClip>();

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Min(0f)]
    [SerializeField] private float volumeVariation;

    [Header("Pitch")]
    [SerializeField] private Vector2 pitchRange = new(0.94f, 1.06f);

    public float RandomVolume => Mathf.Clamp01(
        volume + UnityEngine.Random.Range(-volumeVariation, volumeVariation));

    public float RandomPitch
    {
        get
        {
            float minimum = Mathf.Min(pitchRange.x, pitchRange.y);
            float maximum = Mathf.Max(pitchRange.x, pitchRange.y);
            return UnityEngine.Random.Range(minimum, maximum);
        }
    }

    public bool TryChooseClip(
        int previousIndex,
        out AudioClip clip,
        out int selectedIndex)
    {
        clip = null;
        selectedIndex = -1;

        if (clips == null)
            return false;

        int usableCount = 0;
        int onlyUsableIndex = -1;
        bool canExcludePrevious = false;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            usableCount++;
            onlyUsableIndex = i;
            canExcludePrevious |= i == previousIndex;
        }

        if (usableCount == 0)
            return false;

        if (usableCount == 1)
        {
            selectedIndex = onlyUsableIndex;
            clip = clips[selectedIndex];
            return true;
        }

        int eligibleCount = usableCount - (canExcludePrevious ? 1 : 0);
        int selection = UnityEngine.Random.Range(0, eligibleCount);

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null || i == previousIndex)
                continue;

            if (selection-- > 0)
                continue;

            selectedIndex = i;
            clip = clips[i];
            return true;
        }

        return false;
    }
}
