using UnityEngine;
using UnityEngine.VFX;

public class Torch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect flameVfx;
    [SerializeField] private ParticleSystem flameParticles;

    private Light flameLight;

    [Header("Flame Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color flameColor = Color.white;
    [SerializeField] private LanternColor lightColor = LanternColor.ORANGE;

    public LanternColor LightColor => lightColor;

    private static readonly int VfxColorId = Shader.PropertyToID(ShaderProperty.FIRE_COLOR);

    private void Awake()
    {
        if (flameParticles != null)
            flameLight = flameParticles.GetComponent<Light>();

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
}
