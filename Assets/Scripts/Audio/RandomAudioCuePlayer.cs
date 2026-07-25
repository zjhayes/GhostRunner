using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioCuePlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private readonly Dictionary<RandomAudioCue, int> previousIndices = new();

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Play(RandomAudioCue cue)
    {
        if (cue == null || audioSource == null)
            return;

        int previousIndex = previousIndices.TryGetValue(cue, out int index)
            ? index
            : -1;

        if (!cue.TryChooseClip(previousIndex, out AudioClip clip, out int selectedIndex))
            return;

        audioSource.pitch = cue.RandomPitch;
        audioSource.PlayOneShot(clip, cue.RandomVolume);
        previousIndices[cue] = selectedIndex;
    }
}
