using UnityEngine;

public class GhostScatter : GhostBehaviour
{
    private void Start()
    {
        Context.Ghost.OnTriggerEnter += OnTrigger;
    }

    private void OnTrigger(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        if (node == null) return;

        if (!enabled || Context.Frightened.enabled) return;

        int count = node.Edges.Count;
        if (count == 0) return;

        Cardinal opposite = CardinalUtil.Opposite(Context.Ghost.Movement.Direction);

        bool avoidReverse = count > 1;

        Cardinal chosen = default;
        bool hasChosen = false;

        for (int attempt = 0; attempt < 6; attempt++)
        {
            int pickIndex = Random.Range(0, count);
            int i = 0;

            foreach (Cardinal d in node.Edges.Keys)
            {
                if (i == pickIndex)
                {
                    if (avoidReverse && d == opposite)
                        break; // reroll
                    chosen = d;
                    hasChosen = true;
                    break;
                }
                i++;
            }

            if (hasChosen) break;
        }

        if (!hasChosen)
        {
            foreach (Cardinal d in node.Edges.Keys)
            {
                if (avoidReverse && d == opposite)
                    continue;
                    
                chosen = d;
                hasChosen = true;
                break;
            }
        }

        if (hasChosen)
            Context.Ghost.Movement.SetDirection(chosen);
    }

    private void OnDisable()
    {
        Context.Chase.Enable();
    }
}
