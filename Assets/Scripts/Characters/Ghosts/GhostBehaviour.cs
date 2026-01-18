using UnityEngine;

[RequireComponent (typeof(GhostContext))]
public abstract class GhostBehaviour : MonoBehaviour
{
    [SerializeField] protected float duration;

    public GhostContext Context {  get; private set; }

    private void Awake()
    {
        Context = GetComponent<GhostContext> ();
        enabled = false;
    }

    public virtual void Enable()
    {
       Enable(duration);
    }

    public virtual void Enable(float duration)
    {
        enabled = true;

        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        enabled = false;
        CancelInvoke();
    }
}
