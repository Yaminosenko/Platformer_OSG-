using UnityEngine;

namespace FigmentGames
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Converts a Vector2 to a Vector3.
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vector, float zValue)
        {
            return new Vector3(vector.x, vector.y, zValue);
        }

        /// <summary>
        /// Converts a Vector2 to a Vector2Int.
        /// </summary>
        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }


        /// <summary>
        /// Returns the vector with the given X value.
        /// </summary>
        public static Vector2 XValue(this Vector2 vector, float xValue)
        {
            vector.x = xValue;
            return vector;
        }

        /// <summary>
        /// Returns the vector with the given Y value.
        /// </summary>
        public static Vector2 YValue(this Vector2 vector, float yValue)
        {
            vector.y = yValue;
            return vector;
        }


        /// <summary>
        /// Returns the vector with offseted X value.
        /// </summary>
        public static Vector2 XOffset(this Vector2 vector, float xOffset)
        {
            vector.x += xOffset;
            return vector;
        }

        /// <summary>
        /// Returns the vector with offseted Y value.
        /// </summary>
        public static Vector2 YOffset(this Vector2 vector, float yOffset)
        {
            vector.y += yOffset;
            return vector;
        }


        /// <summary>
        /// Retrives the vector with positive values only.
        /// </summary>
        public static Vector2 Positive(this Vector2 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);

            return vector;
        }

        /// <summary>
        /// Retrives the vector with negative values only.
        /// </summary>
        public static Vector2 Negative(this Vector2 vector)
        {
            return -vector.Positive();
        }


        /// <summary>
        /// Divides the vector with another one. Null values of otherVector will not be considered (replaced by 1).
        /// </summary>
        public static Vector2 Divide(this Vector2 vector, Vector2 otherVector)
        {
            return new Vector2(
                otherVector.x == 0f ? vector.x : vector.x / otherVector.x,
                otherVector.y == 0f ? vector.y : vector.y / otherVector.y);
        }


        /// <summary>
        /// Clamps the minimum values of the vector.
        /// </summary>
        public static Vector2 Min(this Vector2 vector, float minX, float minY)
        {
            return new Vector2(
                vector.x < minX ? minX : vector.x,
                vector.y < minY ? minY : vector.y);
        }

        /// <summary>
        /// Clamps the maximum values of the vector.
        /// </summary>
        public static Vector2 Max(this Vector2 vector, float maxX, float maxY)
        {
            return new Vector2(
                vector.x > maxX ? maxX : vector.x,
                vector.y > maxY ? maxY : vector.y);
        }


        /// <summary>
        /// Retrieves X divided by Y.
        /// </summary>
        public static float Ratio(this Vector2 vector)
        {
            return vector.x / vector.y;
        }


        /// <summary>
        /// Clamps the Vector2 in-between the given min and max vectors.
        /// </summary>
        public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
        {
            return new Vector2(
                    Mathf.Clamp(vector.x, min.x, max.x),
                    Mathf.Clamp(vector.y, min.y, max.y));
        }
    }
}