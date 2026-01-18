using UnityEngine;

[RequireComponent(typeof(GhostHome))]
[RequireComponent(typeof(GhostScatter))]
[RequireComponent(typeof(GhostChase))]
[RequireComponent(typeof(GhostFrightened))]
public class GhostContext : MonoBehaviour
{
    [SerializeField] Transform homeTransform;
    [SerializeField] Transform exitTransform;
    [SerializeField] GhostBehaviour initialBehaviour;

    public Transform HomeTransform => homeTransform;
    public Transform ExitTransform => exitTransform;
    public GhostHome Home { get; private set; }
    public GhostScatter Scatter { get; private set; }
    public GhostChase Chase { get; private set; }
    public GhostFrightened Frightened { get; private set; }
    public Ghost Ghost { get; set; }


    private void Awake()
    {
        Home = GetComponent<GhostHome>();
        Scatter = GetComponent<GhostScatter>();
        Chase = GetComponent<GhostChase>();
        Frightened = GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        Frightened.Disable();
        Chase.Disable();
        Scatter.Enable();

        if (Home != initialBehaviour)
        {
            Home.Disable();
        }

        if (initialBehaviour != null)
        {
            initialBehaviour.Enable();
        }
    }
}
