using UnityEngine;

public class Conversion : MonoBehaviour
{
    public static Vector2 QuantizeToCardinal(Vector2 dir)
    {
        if (dir == Vector2.zero) return Vector2.zero;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0f);

        return new Vector2(0f, Mathf.Sign(dir.y));
    }
}
