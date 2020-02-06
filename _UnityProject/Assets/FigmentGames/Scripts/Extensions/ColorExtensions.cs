using UnityEngine;

namespace FigmentGames
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns the specified RGB color with alpha value.
        /// </summary>
        public static Color Alpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// Returns the specified RGB color with relative alpha value.
        /// </summary>
        public static Color AlphaRelative(this Color color, float alpha)
        {
            color.a *= alpha;
            return color;
        }
    }
}