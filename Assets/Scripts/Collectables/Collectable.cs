using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] int points = 10;
    public int Points { get { return points; } }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.PLAYER))
        {
            Collect();
        }
    }

    protected virtual void Collect()
    {
        GameManager.Instance.OnCollect(this);
    }
}
