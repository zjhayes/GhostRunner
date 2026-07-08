using UnityEngine;

[RequireComponent(typeof(Gremlin))]
public class GremlinLanternFear : GameBehaviour
{
    [SerializeField] private Gremlin gremlin;

    [Header("Frighten Detection")]
    [SerializeField] private float frightenedDistance = 4f;
    [SerializeField] private float frightenedDuration = 3f;

    private void Awake()
    {
        if (!gremlin)
            gremlin = GetComponent<Gremlin>();
    }

    private void Update()
    {
        if (IsNearPlayerLantern())
            gremlin.Frighten(frightenedDuration);
    }

    private bool IsNearPlayerLantern()
    {
        if (GameManager.Player == null)
            return false;

        LanternController lantern = GameManager.Player.Lantern;

        if (lantern == null || lantern.Light == null)
            return false;

        Light light = lantern.Light;

        if (!light.enabled || light.intensity <= 0f)
            return false;

        Vector2 lightPosition = light.transform.position;
        Vector2 gremlinPosition = transform.position;

        float distance = frightenedDistance > 0f ? frightenedDistance : light.range;
        return (gremlinPosition - lightPosition).sqrMagnitude <= distance * distance;
    }
}
