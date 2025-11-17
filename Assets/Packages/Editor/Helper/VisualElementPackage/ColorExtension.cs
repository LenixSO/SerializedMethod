using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    /// <summary>
    /// Creates a shade of gray.
    /// </summary>
    /// <param name="insensity">The closest to 1, the whiter it is.</param>
    public static Color GrayShade(float insensity)
    {
        insensity = Mathf.Clamp01(insensity);

        return new Color(insensity, insensity, insensity, 1);
    }

    public static Color TransparentBlack(float transparency) => new(0, 0, 0, transparency);

    public static Color Transparent(this Color color, float transparency) => new(color.r, color.g, color.b, transparency);

    public static Color Inverted(this Color color) => InvertColor(color);

    public static Color InvertColor(Color color)
    {
        Color invertedColor = color;
        invertedColor.r = 1 - invertedColor.r;
        invertedColor.g = 1 - invertedColor.g;
        invertedColor.b = 1 - invertedColor.b;

        return invertedColor;
    }
    
    public static Color ContrastGray(Color color)
    {
        float anchor = (1 - color.g) * 2 - (color.b + color.r);
        return GrayShade(anchor);
    }
}
