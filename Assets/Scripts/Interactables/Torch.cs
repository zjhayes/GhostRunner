using UnityEngine;
using UnityEngine.VFX;

public class Torch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect flameVfx;
    [SerializeField] private ParticleSystem flameParticles;

    [Header("Flame Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color flameColor = Color.white;
    [SerializeField] private LanternColor lightColor = LanternColor.ORANGE;

    public LanternColor LightColor => lightColor;

    private static readonly int VfxColorId = Shader.PropertyToID(ShaderProperty.FIRE_COLOR);

    private void Awake()
    {
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
        main.startColor = LanternColorUtil.ToColor(color);
    }

    public void Light()
    {
        if (flameVfx != null)
            flameVfx.Play();

        if (flameParticles != null)
            flameParticles.Play();
    }

    public void Extinguish()
    {
        if (flameVfx != null)
            flameVfx.Stop();

        if (flameParticles != null)
            flameParticles.Stop();
    }
}
