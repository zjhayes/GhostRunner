using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Barrier Settings")]
    [SerializeField] private Renderer barrierRenderer;
    [SerializeField] private Collider barrierCollider;

    [Header("Pass Through Settings")]
    [SerializeField] LanternColor requiredColor;
    [SerializeField] float passThroughThreshold = 0.85f;

    private Material barrierMaterial;
    private LanternController lantern;
    private float currentVisibility = 0f;

    private static readonly int LanternPosId = Shader.PropertyToID(ShaderProperty.LANTERN_POS);

    private static readonly int LanternRadiusId = Shader.PropertyToID(ShaderProperty.LANTERN_RADIUS);

    private static readonly int LanternColorId = Shader.PropertyToID(ShaderProperty.LANTERN_COLOR);
    private static readonly int RequiredColorId = Shader.PropertyToID(ShaderProperty.REQUIRED_LANTERN_COLOR);

    public bool IsPassable => currentVisibility >= passThroughThreshold;


    private void Awake()
    {
        barrierMaterial = barrierRenderer.material;
        lantern = GameManager.Instance.Player.Lantern;
        
        barrierMaterial.SetFloat(RequiredColorId, (float)requiredColor);
    }

    void Update()
    {
        if (!lantern) return;
        
        Shader.SetGlobalVector(LanternPosId, lantern.transform.position);
        Shader.SetGlobalFloat(LanternRadiusId, lantern.Light.range);
        Shader.SetGlobalFloat(LanternColorId, (float)lantern.Color);

        float distance = Vector3.Distance(
            lantern.transform.position,
            transform.position
        );

        float distanceFactor = Mathf.Clamp01(
            1f - (distance / lantern.Light.range)
        );

        float colorMatch =
            lantern.Color == requiredColor ? 1f : 0f;

        currentVisibility = distanceFactor * colorMatch;

        barrierCollider.enabled = currentVisibility < passThroughThreshold;
    }

}
