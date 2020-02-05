using UnityEngine;

namespace FigmentGames
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Returns a Z-flatten vector.
        /// </summary>
        public static Vector3 Vector3XY(this Vector3 vector, bool normalize = false)
        {
            vector.z = 0;

            if (normalize)
                vector = vector.normalized;

            return vector;
        }

        /// <summary>
        /// Returns a Y-flatten vector.
        /// </summary>
        public static Vector3 Vector3XZ(this Vector3 vector, bool normalize = false)
        {
            vector.y = 0;

            if (normalize)
                vector = vector.normalized;

            return vector;
        }

        /// <summary>
        /// Returns a X-flatten vector.
        /// </summary>
        public static Vector3 Vector3YZ(this Vector3 vector, bool normalize = false)
        {
            vector.x = 0;

            if (normalize)
                vector = vector.normalized;

            return vector;
        }


        /// <summary>
        /// Returns the vector with the given X value.
        /// </summary>
        public static Vector3 XValue(this Vector3 vector, float xValue)
        {
            vector.x = xValue;
            return vector;
        }

        /// <summary>
        /// Returns the vector with the given Y value.
        /// </summary>
        public static Vector3 YValue(this Vector3 vector, float yValue)
        {
            vector.y = yValue;
            return vector;
        }

        /// <summary>
        /// Returns the vector with the given Z value.
        /// </summary>
        public static Vector3 ZValue(this Vector3 vector, float zValue)
        {
            vector.z = zValue;
            return vector;
        }


        /// <summary>
        /// Returns the vector with offseted X value.
        /// </summary>
        public static Vector3 XOffset(this Vector3 vector, float xOffset)
        {
            vector.x += xOffset;
            return vector;
        }

        /// <summary>
        /// Returns the vector with offseted Y value.
        /// </summary>
        public static Vector3 YOffset(this Vector3 vector, float yOffset)
        {
            vector.y += yOffset;
            return vector;
        }

        /// <summary>
        /// Returns the vector with offseted Z value.
        /// </summary>
        public static Vector3 ZOffset(this Vector3 vector, float zOffset)
        {
            vector.z += zOffset;
            return vector;
        }


        /// <summary>
        /// Retrives the vector with positive values only.
        /// </summary>
        public static Vector3 Positive(this Vector3 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);

            return vector;
        }

        /// <summary>
        /// Retrives the vector with negative values only.
        /// </summary>
        public static Vector3 Negative(this Vector3 vector)
        {
            return -vector.Positive();
        }


        /// <summary>
        /// Multiplies the vector with another one.
        /// </summary>
        public static Vector3 Multiply(this Vector3 vector, Vector3 otherVector)
        {
            return new Vector3(
                vector.x * otherVector.x,
                vector.y * otherVector.y,
                vector.z * otherVector.z);
        }

        /// <summary>
        /// Divides the vector with another one. Null values of otherVector will not be considered (replaced by 1).
        /// </summary>
        public static Vector3 Divide(this Vector3 vector, Vector3 otherVector)
        {
            return new Vector3(
                otherVector.x == 0f ? vector.x : vector.x / otherVector.x,
                otherVector.y == 0f ? vector.y : vector.y / otherVector.y,
                otherVector.z == 0f ? vector.z : vector.z / otherVector.z);
        }


        /// <summary>
        /// Snaps the vector to the given Vector3 spacing.
        /// </summary>
        public static Vector3 Snap(this Vector3 vector, Vector3 snap, Vector3 offset = default)
        {
            return new Vector3(
                Mathf.RoundToInt((vector.x + offset.x) / snap.x) * snap.x,
                Mathf.RoundToInt((vector.y + offset.y) / snap.y) * snap.y,
                Mathf.RoundToInt((vector.z + offset.z) / snap.z) * snap.z) - offset;
        }


        /// <summary>
        /// Clamps the minimum values of the vector.
        /// </summary>
        public static Vector3 Min(this Vector3 vector, float minX, float minY, float minZ)
        {
            return new Vector3(
                vector.x < minX ? minX : vector.x,
                vector.y < minY ? minY : vector.y,
                vector.z < minZ ? minZ : vector.z);
        }

        /// <summary>
        /// Clamps the maximum values of the vector.
        /// </summary>
        public static Vector3 Max(this Vector3 vector, float maxX, float maxY, float maxZ)
        {
            return new Vector3(
                vector.x > maxX ? maxX : vector.x,
                vector.y > maxY ? maxY : vector.y,
                vector.z > maxZ ? maxZ : vector.z);
        }

        /// <summary>
        /// Clamps the Vector3 in-between the given min and max vectors.
        /// </summary>
        public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
        {
            return new Vector3(
                    Mathf.Clamp(vector.x, min.x, max.x),
                    Mathf.Clamp(vector.y, min.y, max.y),
                    Mathf.Clamp(vector.z, min.z, max.z));
        }
    }
}