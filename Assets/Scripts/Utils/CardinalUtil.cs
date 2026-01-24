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

    public static Vector3Int ToCellOffset(Cardinal c) => c switch
    {
        Cardinal.North => Vector3Int.up,
        Cardinal.South => Vector3Int.down,
        Cardinal.East  => Vector3Int.right,
        Cardinal.West  => Vector3Int.left,
        _ => Vector3Int.zero
    };


    public static Cardinal Opposite(Cardinal c) => c switch
    {
        Cardinal.North => Cardinal.South,
        Cardinal.South => Cardinal.North,
        Cardinal.East  => Cardinal.West,
        Cardinal.West  => Cardinal.East,
        _ => c
    };

    public static readonly Cardinal[] Cardinals =
    {
        Cardinal.North,
        Cardinal.South,
        Cardinal.West,
        Cardinal.East
    };

    public static bool IsHorizontal(Cardinal c) => c == Cardinal.East || c == Cardinal.West;
    public static bool IsVertical(Cardinal c) => c == Cardinal.North || c == Cardinal.South;

    public static bool IsOpposite(Cardinal a, Cardinal b) => Opposite(a) == b;
}
