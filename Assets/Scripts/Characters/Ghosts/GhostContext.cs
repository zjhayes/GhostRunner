using UnityEngine;

[RequireComponent(typeof(GhostScatter))]
[RequireComponent(typeof(GhostChase))]
[RequireComponent(typeof(GhostIdle))]
public class GhostContext : MonoBehaviour
{
    [SerializeField] GhostBehaviour initialBehaviour;

    public GhostScatter Scatter { get; private set; }
    public GhostChase Chase { get; private set; }
    public GhostIdle Idle { get; private set; }
    public Ghost Ghost { get; set; }
    public bool IsIdle { get; private set; }
    public bool SuppressBehaviourTransitions { get; private set; }

    protected virtual void Awake()
    {
        Scatter = GetComponent<GhostScatter>();
        Chase = GetComponent<GhostChase>();
        Idle = GetComponent<GhostIdle>();
    }

    protected virtual void Start()
    {
        ResetState();
    }

    public virtual void ResetState()
    {
        IsIdle = false;
        DisableNormalBehaviours();
        Idle.Disable();

        if (initialBehaviour != null)
            initialBehaviour.Enable();
        else
            Scatter.Enable();
    }

    public virtual void EnterScatter()
    {
        if (IsIdle)
            return;

        DisableNormalBehaviours();
        Scatter.Enable();
    }

    public virtual void EnterIdle()
    {
        if (IsIdle)
            return;

        IsIdle = true;
        DisableNormalBehaviours();
        Idle.Enable();
    }

    protected void DisableNormalBehaviours()
    {
        SuppressBehaviourTransitions = true;

        Chase.Disable();
        Scatter.Disable();

        SuppressBehaviourTransitions = false;
    }
}
