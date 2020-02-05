using UnityEngine;

namespace FigmentGames
{
    public static class RectExtensions
    {
        /// <summary>
        /// Retrieves a rect with a specific X value.
        /// </summary>
        public static Rect SetX(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        /// <summary>
        /// Offsets X value of Rect.
        /// </summary>
        public static Rect XOffset(this Rect rect, float offset)
        {
            rect.x += offset;
            return rect;
        }

        /// <summary>
        /// Retrieves a rect with a specific Y value.
        /// </summary>
        public static Rect SetY(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        /// <summary>
        /// Offsets Y value of Rect.
        /// </summary>
        public static Rect YOffset(this Rect rect, float offset)
        {
            rect.y += offset;
            return rect;
        }

        /// <summary>
        /// Retrieves a rect with a specific width.
        /// </summary>
        public static Rect SetWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        /// <summary>
        /// Offsets Y value of Rect.
        /// </summary>
        public static Rect WidthOffset(this Rect rect, float offset)
        {
            rect.width += offset;
            return rect;
        }

        /// <summary>
        /// Retrieves a rect with a specific width.
        /// </summary>
        public static Rect SetHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        /// <summary>
        /// Offsets Y value of Rect.
        /// </summary>
        public static Rect HeightOffset(this Rect rect, float offset)
        {
            rect.height += offset;
            return rect;
        }


        /// <summary>
        /// Adds Rect values.
        /// </summary>
        public static Rect Add(this Rect rect, Rect otherRect)
        {
            rect.x += otherRect.x;
            rect.y += otherRect.y;
            rect.width += otherRect.width;
            rect.height += otherRect.height;
            return rect;
        }
    }
}