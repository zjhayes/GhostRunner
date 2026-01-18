using UnityEngine;

public class Passage : MonoBehaviour
{
    [SerializeField] Transform connection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3  position = other.transform.position;
        position.x = connection.position.x;
        position.y = connection.position.y;
        other.transform.position = position;
    }
}
