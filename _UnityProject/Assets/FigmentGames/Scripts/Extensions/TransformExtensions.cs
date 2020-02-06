using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FigmentGames
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Returns the specified RGB color with alpha value.
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Destroys all children of the transform.
        /// </summary>
        public static void DestroyImmediateAllChildren(this Transform transform)
        {
            int i = transform.childCount;
            while (--i >= 0)
            {
                GameObject o = transform.GetChild(i).gameObject;
                Object.DestroyImmediate(o);
            }
        }

        /// <summary>
        /// Destroys all children of the transform.
        /// </summary>
        public static void DestroyAllChildren(this Transform transform)
        {
            int i = transform.childCount;
            while (--i >= 0)
            {
                GameObject o = transform.GetChild(i).gameObject;
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying)
                {
                    Object.DestroyImmediate(o);
                }
                else
                {
                    Object.Destroy(o);
                }
#else
                Object.Destroy(o);
#endif
            }
        }
    }
}