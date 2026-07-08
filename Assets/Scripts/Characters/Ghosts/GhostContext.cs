using UnityEngine;

[RequireComponent(typeof(GhostScatter))]
[RequireComponent(typeof(GhostChase))]
public class GhostContext : MonoBehaviour
{
    [SerializeField] GhostBehaviour initialBehaviour;

    public GhostScatter Scatter { get; private set; }
    public GhostChase Chase { get; private set; }
    public Ghost Ghost { get; set; }
    public bool SuppressBehaviourTransitions { get; private set; }

    protected virtual void Awake()
    {
        Scatter = GetComponent<GhostScatter>();
        Chase = GetComponent<GhostChase>();
    }

    protected virtual void Start()
    {
        ResetState();
    }

    public virtual void ResetState()
    {
        DisableNormalBehaviours();

        if (initialBehaviour != null)
            initialBehaviour.Enable();
        else
            Scatter.Enable();
    }

    protected void DisableNormalBehaviours()
    {
        SuppressBehaviourTransitions = true;

        Chase.Disable();
        Scatter.Disable();

        SuppressBehaviourTransitions = false;
    }
}
