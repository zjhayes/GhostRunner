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

    public static Vector2 ToVector(Cardinal c) => c switch
    {
        Cardinal.North => Vector2.up,
        Cardinal.South => Vector2.down,
        Cardinal.East  => Vector2.right,
        Cardinal.West  => Vector2.left,
        _ => Vector2.zero
    };

    public static Cardinal Opposite(Cardinal c) => c switch
    {
        Cardinal.North => Cardinal.South,
        Cardinal.South => Cardinal.North,
        Cardinal.East  => Cardinal.West,
        Cardinal.West  => Cardinal.East,
        _ => c
    };

    public static bool IsHorizontal(Cardinal c) => c == Cardinal.East || c == Cardinal.West;
    public static bool IsVertical(Cardinal c) => c == Cardinal.North || c == Cardinal.South;

    public static bool IsOpposite(Cardinal a, Cardinal b) => Opposite(a) == b;
}
