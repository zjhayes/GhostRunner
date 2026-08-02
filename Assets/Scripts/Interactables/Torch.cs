using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Torch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect flameVfx;
    [SerializeField] private ParticleSystem flameParticles;
    [SerializeField] private AudioSource flameWhooshSource;
    [SerializeField] private AudioSource achievementSource;

    private Light flameLight;

    [Header("Interaction Feedback")]
    [Min(0f)]
    [SerializeField] private float intensityPulseDuration = 0.45f;
    [Min(1f)]
    [SerializeField] private float intensityPulseMultiplier = 1.5f;
    [Min(1f)]
    [SerializeField] private float flameSpawnRateMultiplier = 3f;
    [Min(1f)]
    [SerializeField] private float flameScaleMultiplier = 1.3f;

    private Coroutine interactionPulse;
    private float baseLightIntensity;
    private float baseFlameSpawnRate;
    private bool hasFlameSpawnRate;
    private Vector3 baseFlameScale;

    [Header("Flame Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color flameColor = Color.white;
    [SerializeField] private LanternColor lightColor = LanternColor.ORANGE;

    public LanternColor LightColor => lightColor;

    private static readonly int VfxColorId = Shader.PropertyToID(ShaderProperty.FIRE_COLOR);
    private static readonly int VfxFlameRateId = Shader.PropertyToID("FlameRate");

    private void Awake()
    {
        if (flameParticles != null)
            flameLight = flameParticles.GetComponent<Light>();

        if (flameLight != null)
            baseLightIntensity = flameLight.intensity;

        hasFlameSpawnRate = flameVfx != null && flameVfx.HasFloat(VfxFlameRateId);
        if (hasFlameSpawnRate)
            baseFlameSpawnRate = flameVfx.GetFloat(VfxFlameRateId);

        if (flameVfx != null)
            baseFlameScale = flameVfx.transform.localScale;

        ApplyFlameColor(flameColor);
        ApplyParticleColor(lightColor);
    }

    public void SetFlameColor(Color color)
    {
        flameColor = color;
        ApplyFlameColor(color);
    }

    public void SetParticleLightColor(LanternColor color)
    {
        lightColor = color;
        ApplyParticleColor(color);
    }

    public void PlayInteractionFeedback()
    {
        if (flameWhooshSource != null)
            flameWhooshSource.Play();

        if (achievementSource != null)
            achievementSource.Play();

        if (intensityPulseDuration <= 0f || (flameLight == null && !hasFlameSpawnRate))
            return;

        if (interactionPulse != null)
            StopCoroutine(interactionPulse);

        ResetInteractionPulse();
        interactionPulse = StartCoroutine(PulseInteractionFeedback());
    }

    private IEnumerator PulseInteractionFeedback()
    {
        float elapsed = 0f;

        while (elapsed < intensityPulseDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / intensityPulseDuration);
            float pulse = Mathf.Sin(progress * Mathf.PI);

            if (flameLight != null)
                flameLight.intensity = baseLightIntensity * Mathf.Lerp(1f, intensityPulseMultiplier, pulse);

            if (hasFlameSpawnRate)
                flameVfx.SetFloat(
                    VfxFlameRateId,
                    baseFlameSpawnRate * Mathf.Lerp(1f, flameSpawnRateMultiplier, pulse)
                );

            if (flameVfx != null)
                flameVfx.transform.localScale = baseFlameScale * Mathf.Lerp(1f, flameScaleMultiplier, pulse);

            yield return null;
        }

        ResetInteractionPulse();
        interactionPulse = null;
    }

    private void ResetInteractionPulse()
    {
        if (flameLight != null)
            flameLight.intensity = baseLightIntensity;

        if (hasFlameSpawnRate)
            flameVfx.SetFloat(VfxFlameRateId, baseFlameSpawnRate);

        if (flameVfx != null)
            flameVfx.transform.localScale = baseFlameScale;
    }

    private void ApplyFlameColor(Color color)
    {
        if (flameVfx != null)
        {
            flameVfx.SetVector4(VfxColorId, color);
        }
    }

    private void ApplyParticleColor(LanternColor color)
    {
        if (flameParticles == null)
            return;

        var main = flameParticles.main;
        Color resolvedColor = LanternColorUtil.ToColor(color);
        main.startColor = resolvedColor;

        if (flameLight != null)
            flameLight.color = resolvedColor;
    }

    public void Light()
    {
        if (flameVfx != null)
            flameVfx.Play();

        if (flameParticles != null)
            flameParticles.Play();

        if (flameLight != null)
            flameLight.enabled = true;
    }

    public void Extinguish()
    {
        if (flameVfx != null)
            flameVfx.Stop();

        if (flameParticles != null)
            flameParticles.Stop();

        if (flameLight != null)
            flameLight.enabled = false;
    }

    private void OnDisable()
    {
        if (interactionPulse == null)
            return;

        StopCoroutine(interactionPulse);
        interactionPulse = null;
        ResetInteractionPulse();
    }
}
