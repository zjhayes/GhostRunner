using UnityEngine;

public class LanternController : MonoBehaviour
{
    [SerializeField] private MovementManager movement;
    [SerializeField] private Light lanternLight;

    [Header("Lantern Sockets")]
    [SerializeField] private Transform eastSocket;
    [SerializeField] private Transform westSocket;
    [SerializeField] private Transform northSocket;
    [SerializeField] private Transform southSocket;

    private void Awake()
    {
        if (!movement) movement = GetComponentInParent<MovementManager>();
        if (!lanternLight) lanternLight = GetComponentInChildren<Light>();
    }

    private void OnEnable()
    {
        if (movement == null) return;
        movement.OnDirectionChanged += HandleDirectionChanged;
    }

    private void OnDisable()
    {
        if (movement == null) return;
        movement.OnDirectionChanged -= HandleDirectionChanged;
    }

    private void Start()
    {
        if (movement != null)
            ApplySocket(movement.Direction);
    }

    private void HandleDirectionChanged(Cardinal dir)
    {
        ApplySocket(dir);
    }

    private void ApplySocket(Cardinal dir)
    {
        if (!lanternLight) return;

        Transform socket = dir switch
        {
            Cardinal.East  => eastSocket,
            Cardinal.West  => westSocket,
            Cardinal.North => northSocket,
            Cardinal.South => southSocket,
            _ => eastSocket
        };

        if (!socket) return;

        lanternLight.transform.SetPositionAndRotation(socket.position, socket.rotation);
    }
}
