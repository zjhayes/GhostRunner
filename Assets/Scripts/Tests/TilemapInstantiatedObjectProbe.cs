using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInstantiatedObjectProbe : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    void Start()
    {
        Probe();
    }

    [ContextMenu("Probe Instantiated Objects")]
    public void Probe()
    {
        if (!tilemap) { Debug.LogError("Assign a Tilemap."); return; }

        var bounds = tilemap.cellBounds;
        int tiles = 0, found = 0;

        foreach (var cell in bounds.allPositionsWithin)
        {
            // Skip empty cells
            if (!tilemap.HasTile(cell)) continue;
            tiles++;

            // Instance spawned by tile at this cell (only if the tile spawns a GameObject)
            GameObject go = tilemap.GetInstantiatedObject(cell);

            if (go != null)
            {
                found++;
                Debug.Log($"Cell {cell}: {go.name}", go);

                // Example: see if it has a Node component
                var node = go.GetComponent<Node>();
                if (node != null)
                    Debug.Log($"  -> has Node component on {cell}");
            }
        }

        Debug.Log($"Scanned {tiles} tiles. Found {found} instantiated objects.");
    }
}
