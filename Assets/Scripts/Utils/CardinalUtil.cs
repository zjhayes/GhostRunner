using UnityEngine;

public enum Cardinal { North, South, East, West }

public static class CardinalUtil
{
    public static Cardinal FromVector(Vector2 dir, Cardinal fallback)
    {
        if (dir == Vector2.zero) return fallback;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x >= 0 ? Cardinal.East : Cardinal.West;

        return dir.y >= 0 ? Cardinal.North : Cardinal.South;
    }
}
