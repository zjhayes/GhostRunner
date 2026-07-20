using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterFootsteps : MonoBehaviour
{
    [Header("Surface Detection")]
    [SerializeField] private Transform surfaceCheckPoint;
    [SerializeField] private LayerMask surfaceLayer;

    [Tooltip("Direction from the character toward the floor.")]
    [SerializeField] private Vector3 surfaceDirection = Vector3.forward;

    [Min(0.01f)]
    [SerializeField] private float surfaceCheckDistance = 1f;

    [Min(0f)]
    [SerializeField] private float surfaceCheckOffset = 0.1f;

    [Header("Sounds")]
    [SerializeField] private FootstepAudioProfile audioProfile;

    [Header("Movement")]
    [SerializeField] private MovementManager movementManager;

    private AudioSource audioSource;

    private AudioClip[] previousCollection;
    private int previousClipIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (movementManager == null)
            movementManager = GetComponentInParent<MovementManager>();

        if (surfaceCheckPoint == null)
            surfaceCheckPoint = transform;
    }

    /// Called by an Animation Event at each foot-contact frame.
    public void PlayFootstep()
    {
        if (movementManager == null || !movementManager.IsMoving)
            return;

        FootstepSurface surface = audioProfile != null
            ? audioProfile.Resolve(DetectSurfaceType())
            : null;

        if (surface == null)
            return;

        PlayRandomClip(surface, movementManager.IsRunning);
    }

    private FootstepSurfaceType? DetectSurfaceType()
    {
        if (surfaceCheckPoint == null)
            return null;

        if (surfaceDirection.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning(
                "CharacterFootsteps has no valid surface direction.",
                this
            );

            return null;
        }

        Vector3 direction = surfaceDirection.normalized;

        // Move the starting point slightly away from the floor,
        // then cast back toward it.
        Vector3 origin =
            surfaceCheckPoint.position -
            direction * surfaceCheckOffset;

        bool hitSurface = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            surfaceCheckDistance,
            surfaceLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!hitSurface)
            return null;

        FootstepSurfaceArea area =
            hit.collider.GetComponentInParent<FootstepSurfaceArea>();

        return area != null ? area.surfaceType : null;
    }

    private void PlayRandomClip(
        FootstepSurface surface,
        bool running)
    {
        AudioClip[] clips = running
            ? surface.runClips
            : surface.walkClips;

        // Use walking sounds when a surface has no run collection.
        if (!HasUsableClips(clips) && running)
            clips = surface.walkClips;

        if (!HasUsableClips(clips))
            return;

        // Reset the previous index when changing clip collections.
        if (clips != previousCollection)
        {
            previousCollection = clips;
            previousClipIndex = -1;
        }

        int index = ChooseUsableClipIndex(clips);

        if (index < 0)
            return;

        AudioClip clip = clips[index];

        audioSource.pitch = Random.Range(
            surface.pitchRange.x,
            surface.pitchRange.y
        );

        float baseVolume = running
            ? surface.runVolume
            : surface.walkVolume;

        float volume = baseVolume +
            Random.Range(
                -surface.volumeVariation,
                surface.volumeVariation
            );

        audioSource.PlayOneShot(
            clip,
            Mathf.Clamp01(volume)
        );

        previousClipIndex = index;
    }

    private bool HasUsableClips(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return false;

        foreach (AudioClip clip in clips)
        {
            if (clip != null)
                return true;
        }

        return false;
    }

    private int ChooseUsableClipIndex(AudioClip[] clips)
    {
        int usableCount = 0;

        foreach (AudioClip clip in clips)
        {
            if (clip != null)
                usableCount++;
        }

        if (usableCount == 0)
            return -1;

        if (usableCount == 1)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                    return i;
            }
        }

        // Try a few random selections first.
        for (int attempt = 0; attempt < 20; attempt++)
        {
            int index = Random.Range(0, clips.Length);

            if (clips[index] != null &&
                index != previousClipIndex)
            {
                return index;
            }
        }

        // Deterministic fallback.
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null &&
                i != previousClipIndex)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnDrawGizmosSelected()
    {
        if (surfaceCheckPoint == null)
            return;

        if (surfaceDirection.sqrMagnitude < 0.0001f)
            return;

        Vector3 direction = surfaceDirection.normalized;

        Vector3 origin =
            surfaceCheckPoint.position -
            direction * surfaceCheckOffset;

        Gizmos.DrawSphere(origin, 0.025f);
        Gizmos.DrawLine(
            origin,
            origin + direction * surfaceCheckDistance
        );
    }
}
