using UnityEngine;

namespace FigmentGames
{
    public static class Vector3IntExtensions
    {
        /// <summary>
        /// Returns a Z-flatten vector.
        /// </summary>
        public static Vector3Int Vector3IntXY(this Vector3Int vector)
        {
            vector.z = 0;
            return vector;
        }

        /// <summary>
        /// Returns a Y-flatten vector.
        /// </summary>
        public static Vector3Int Vector3IntXZ(this Vector3Int vector)
        {
            vector.y = 0;
            return vector;
        }

        /// <summary>
        /// Returns a X-flatten vector.
        /// </summary>
        public static Vector3Int Vector3IntYZ(this Vector3Int vector)
        {
            vector.x = 0;
            return vector;
        }


        /// <summary>
        /// Returns the vector with the specified X value.
        /// </summary>
        public static Vector3Int XValue(this Vector3Int vector, int xValue)
        {
            vector.x = xValue;
            return vector;
        }

        /// <summary>
        /// Returns the vector with the specified X value.
        /// </summary>
        public static Vector3Int YValue(this Vector3Int vector, int yValue)
        {
            vector.y = yValue;
            return vector;
        }

        /// <summary>
        /// Returns the vector with the specified X value.
        /// </summary>
        public static Vector3Int ZValue(this Vector3Int vector, int zValue)
        {
            vector.z = zValue;
            return vector;
        }


        /// <summary>
        /// Returns the vector with offseted X value.
        /// </summary>
        public static Vector3Int XOffset(this Vector3Int vector, int xOffset)
        {
            vector.x += xOffset;
            return vector;
        }

        /// <summary>
        /// Returns the vector with offseted Y value.
        /// </summary>
        public static Vector3Int YOffset(this Vector3Int vector, int yOffset)
        {
            vector.y += yOffset;
            return vector;
        }

        /// <summary>
        /// Returns the vector with offseted Z value.
        /// </summary>
        public static Vector3Int ZOffset(this Vector3Int vector, int zOffset)
        {
            vector.z += zOffset;
            return vector;
        }


        /// <summary>
        /// Retrives the vector with positive values only.
        /// </summary>
        public static Vector3Int Positive(this Vector3Int vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);

            return vector;
        }

        /// <summary>
        /// Retrives the vector with negative values only.
        /// </summary>
        public static Vector3Int Negative(this Vector3Int vector)
        {
            vector.x = -Mathf.Abs(vector.x);
            vector.y = -Mathf.Abs(vector.y);
            vector.z = -Mathf.Abs(vector.z);

            return vector;
        }


        /// <summary>
        /// Multiplies the vector with another one.
        /// </summary>
        public static Vector3Int Multiply(this Vector3Int vector, Vector3Int otherVector)
        {
            return new Vector3Int(
                vector.x * otherVector.x,
                vector.y * otherVector.y,
                vector.z * otherVector.z);
        }

        /// <summary>
        /// Divides the vector with another one. Null values of otherVector will not be considered (replaced by 1).
        /// </summary>
        public static Vector3Int Divide(this Vector3Int vector, Vector3Int otherVector)
        {
            return new Vector3Int(
                otherVector.x == 0f ? vector.x : vector.x / otherVector.x,
                otherVector.y == 0f ? vector.y : vector.y / otherVector.y,
                otherVector.z == 0f ? vector.z : vector.z / otherVector.z);
        }


        /// <summary>
        /// Snaps the vector to the given Vector3Int spacing.
        /// </summary>
        public static Vector3Int Snap(this Vector3Int vector, Vector3Int snap, Vector3Int offset = default)
        {
            return new Vector3Int(
                Mathf.RoundToInt((vector.x + offset.x) / snap.x) * snap.x,
                Mathf.RoundToInt((vector.y + offset.y) / snap.y) * snap.y,
                Mathf.RoundToInt((vector.z + offset.z) / snap.z) * snap.z) - offset;
        }


        /// <summary>
        /// Clamps the minimum values of the vector.
        /// </summary>
        public static Vector3Int Min(this Vector3Int vector, int minX, int minY, int minZ)
        {
            return new Vector3Int(
                vector.x < minX ? minX : vector.x,
                vector.y < minY ? minY : vector.y,
                vector.z < minZ ? minZ : vector.z);
        }

        /// <summary>
        /// Clamps the maximum values of the vector.
        /// </summary>
        public static Vector3Int Max(this Vector3Int vector, int maxX, int maxY, int maxZ)
        {
            return new Vector3Int(
                vector.x > maxX ? maxX : vector.x,
                vector.y > maxY ? maxY : vector.y,
                vector.z > maxZ ? maxZ : vector.z);
        }

        /// <summary>
        /// Clamps the Vector3Int in-between the given min and max vectors.
        /// </summary>
        public static Vector3Int Clamp(this Vector3Int vector, Vector3Int min, Vector3Int max)
        {
            return new Vector3Int(
                    Mathf.Clamp(vector.x, min.x, max.x),
                    Mathf.Clamp(vector.y, min.y, max.y),
                    Mathf.Clamp(vector.z, min.z, max.z));
        }
    }
}