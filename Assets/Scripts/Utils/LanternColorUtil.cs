using UnityEngine;
using System.Collections.Generic;

public enum LanternColor
{
    DEFAULT,
    ORANGE,
}

public static class LanternColorUtil
{
    private static readonly Dictionary<LanternColor, Color> colorMap =
        new Dictionary<LanternColor, Color>
        {
            { LanternColor.DEFAULT, FromHex("#FFFFFF") },
            { LanternColor.ORANGE,  FromHex("#F85D19") },
        };

    public static Color ToColor(this LanternColor lanternColor)
    {
        return colorMap[lanternColor];
    }

    private static Color FromHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        return color;
    }
}
