using UnityEngine;

public class BarrierAction : NodeAction
{
    [Header("Barrier Settings")]
    [SerializeField] private Renderer barrierRenderer;

    [Header("Pass Through Settings")]
    [SerializeField] LanternColor requiredColor;

    private Material barrierMaterial;
    private LanternController lantern;
    private bool isPassable;

    private static readonly int LanternPosId = Shader.PropertyToID(ShaderProperty.LANTERN_POS);
    private static readonly int LanternRadiusId = Shader.PropertyToID(ShaderProperty.LANTERN_RADIUS);
    private static readonly int LanternColorId = Shader.PropertyToID(ShaderProperty.LANTERN_COLOR);
    private static readonly int RequiredColorId = Shader.PropertyToID(ShaderProperty.REQUIRED_LANTERN_COLOR);
    private static readonly int BarrierBaseColorId = Shader.PropertyToID(ShaderProperty.BARRIER_BASE_COLOR);

    public bool IsPassable => isPassable;


    private void Awake()
    {
        barrierMaterial = barrierRenderer.material;
        lantern = GameManager.Instance.Player.Lantern;
        
        barrierMaterial.SetFloat(RequiredColorId, (float)requiredColor);
        barrierMaterial.SetColor(
            BarrierBaseColorId,
            LanternColorUtil.ToColor(requiredColor)
        );
    }

    private void Update()
    {
        if (!lantern) return;

        SetGlobalProperties();

        isPassable = lantern.Color == requiredColor;
    }

    protected override void OnResolvePlayer(PlayerManager player, Cardinal direction, Node node, ActionEdge edge)
    {
        player.Movement.ApplyDirection(direction);

        if (!IsPassable)
        {
            player.Movement.Stop();
        }
    }

    protected override void OnResolveGhost(Ghost ghost, Cardinal direction, Node node, ActionEdge edge)
    {
        /**Cardinal[] exclude = { direction };

        if (ghost.Movement.TryResolveDirection(node, out var chosen, exclude))
        {
            node.ResolveEdge(ghost, chosen);
        }
        else
        {
            // No valid alternatives; pick your fallback.
            ghost.Movement.Stop();
        }*/
        ghost.TurnAround();
    }


    void OnEnable()
    {
        SetGlobalProperties();
    }

    private void SetGlobalProperties()
    {
        Shader.SetGlobalVector(LanternPosId, lantern.transform.position);
        Shader.SetGlobalFloat(LanternRadiusId, lantern.Light.range);
        Shader.SetGlobalFloat(LanternColorId, (float)lantern.Color);
    }
}
