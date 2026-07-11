using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Track")]
    [SerializeField] private AudioClip track;
    [SerializeField] private bool playOnStart = true;

    [Header("Loop")]
    [SerializeField] private bool loop = true;
    [Min(0f)]
    [SerializeField] private float loopDelay = 0f;

    private AudioSource audioSource;
    private Coroutine playbackCoroutine;

    public bool IsPlaying => playbackCoroutine != null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Looping is handled here so a delay can be inserted between plays.
        audioSource.loop = false;
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        Stop();

        AudioClip clip = track != null ? track : audioSource.clip;
        if (clip == null)
        {
            Debug.LogWarning("MusicManager cannot play because no track is assigned.", this);
            return;
        }

        playbackCoroutine = StartCoroutine(PlayTrack(clip));
    }

    public void Stop()
    {
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }

        if (audioSource != null)
            audioSource.Stop();
    }

    private IEnumerator PlayTrack(AudioClip clip)
    {
        do
        {
            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying);

            if (loop && loopDelay > 0f)
                yield return new WaitForSecondsRealtime(loopDelay);
        }
        while (loop);

        playbackCoroutine = null;
    }

    private void OnDisable()
    {
        Stop();
    }
}
