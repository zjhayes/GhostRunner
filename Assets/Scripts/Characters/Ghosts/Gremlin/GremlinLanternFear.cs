using UnityEngine;

[RequireComponent(typeof(Gremlin))]
public class GremlinLanternFear : GameBehaviour
{
    [SerializeField] private Gremlin gremlin;

    [Header("Frighten Detection")]
    [SerializeField, Min(0f)] private float frightenedDistance = 2f;
    [SerializeField] private float frightenedDuration = 3f;

    private void Awake()
    {
        if (!gremlin)
            gremlin = GetComponent<Gremlin>();
    }

    private void Update()
    {
        if (!TryGetActivePlayerLantern(out LanternController lantern))
            return;

        if (lantern.Color == LanternColor.DEFAULT)
        {
            if (IsNearLantern(lantern) && gremlin.IsFacing(GameManager.Player.transform))
                gremlin.Frighten(frightenedDuration);
        }
    }

    private bool TryGetActivePlayerLantern(out LanternController lantern)
    {
        lantern = null;

        if (GameManager.Player == null)
            return false;

        lantern = GameManager.Player.Lantern;

        if (lantern == null || lantern.Light == null)
            return false;

        Light light = lantern.Light;
        return light.enabled && light.intensity > 0f;
    }

    private bool IsNearLantern(LanternController lantern)
    {
        Light light = lantern.Light;

        Vector2 lightPosition = light.transform.position;
        Vector2 gremlinPosition = transform.position;

        return (gremlinPosition - lightPosition).sqrMagnitude
            <= frightenedDistance * frightenedDistance;
    }
}
