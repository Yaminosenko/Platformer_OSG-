using UnityEngine;

namespace FigmentGames
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Converts a Quaternion to a Vector4.
        /// </summary>
        public static Vector4 ToVector4(this Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}