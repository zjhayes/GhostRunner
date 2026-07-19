using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Track")]
    [SerializeField] private AudioClip track;
    [SerializeField] private bool playOnStart = true;
    [Min(0f)]
    [SerializeField] private float initialFadeTime = 0f;

    [Header("Loop")]
    [SerializeField] private bool loop = true;
    [Min(0f)]
    [SerializeField] private float loopDelay = 0f;

    private AudioSource audioSource;
    private Coroutine playbackCoroutine;
    private float targetVolume;

    public bool IsPlaying => playbackCoroutine != null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        targetVolume = audioSource.volume;

        // Looping is handled here so a delay can be inserted between plays.
        audioSource.loop = false;

        if (playOnStart)
            Play(initialFadeTime);
    }

    public void Play()
    {
        Play(initialFadeTime);
    }

    public void Play(float fadeTime)
    {
        Stop();

        AudioClip clip = track != null ? track : audioSource.clip;
        if (clip == null)
        {
            Debug.LogWarning("MusicManager cannot play because no track is assigned.", this);
            return;
        }

        playbackCoroutine = StartCoroutine(PlayTrack(clip, Mathf.Max(0f, fadeTime)));
    }

    public void ChangeTrack(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning($"{nameof(MusicManager)} cannot change to a null track.", this);
            return;
        }

        if (track == clip && IsPlaying)
            return;

        track = clip;
        Play(initialFadeTime);
    }

    public void Stop()
    {
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.volume = targetVolume;
        }
    }

    private IEnumerator PlayTrack(AudioClip clip, float fadeTime)
    {
        bool isFirstPlayback = true;

        do
        {
            audioSource.clip = clip;
            audioSource.volume = isFirstPlayback && fadeTime > 0f ? 0f : targetVolume;
            audioSource.Play();

            if (isFirstPlayback && fadeTime > 0f)
                yield return FadeIn(fadeTime);

            yield return new WaitWhile(() => audioSource.isPlaying);

            if (loop && loopDelay > 0f)
                yield return new WaitForSecondsRealtime(loopDelay);

            isFirstPlayback = false;
        }
        while (loop);

        playbackCoroutine = null;
    }

    private IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && audioSource.isPlaying)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private void OnDisable()
    {
        Stop();
    }
}
