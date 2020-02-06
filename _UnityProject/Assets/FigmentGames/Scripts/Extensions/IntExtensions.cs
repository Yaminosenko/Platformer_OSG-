using UnityEngine;

namespace FigmentGames
{
    public static class IntExtensions
    {
        /// <summary>
        /// Clamps the int value to the given minimum one.
        /// </summary>
        public static int Min(this int value, int min)
        {
            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Clamps the int value to the given maximum one.
        /// </summary>
        public static int Max(this int value, int max)
        {
            if (value > max)
                value = max;

            return value;
        }
    }
}