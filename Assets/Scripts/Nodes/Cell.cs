using UnityEngine;

public abstract class Cell : MonoBehaviour
{
    public Vector3Int Position { get; private set; }

    public void Initialize(Vector3Int cell)
    {
        Position = cell;
    }

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.NODES);
    }
}
