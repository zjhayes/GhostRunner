using UnityEngine;

public abstract class Cell : MonoBehaviour
{
    public Vector3Int Position { get; private set; }

    public void Initialize(Vector3Int cell)
    {
        Position = cell;
    }
}
