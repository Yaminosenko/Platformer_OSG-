using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Returns true is the SerializedProperty has not been created or assigned.
        /// </summary>
        public static bool IsNull(this SerializedProperty serializedProperty)
        {
            return serializedProperty == null;
        }


        /// <summary>
        /// Returns true if all this array elements are expanded.
        /// </summary>
        public static bool IsArrayFullyExpanded(this SerializedProperty serializedProperty)
        {
            if (!serializedProperty.isArray)
                return serializedProperty.isExpanded;

            int count = serializedProperty.arraySize;
            for (int i = 0; i < count; i++)
            {
                if (!serializedProperty.GetArrayElementAtIndex(i).isExpanded)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if all this array elements are shrinked.
        /// </summary>
        public static bool IsArrayFullyCollapsed(this SerializedProperty serializedProperty)
        {
            if (!serializedProperty.isArray)
                return !serializedProperty.isExpanded;

            int count = serializedProperty.arraySize;
            for (int i = 0; i < count; i++)
            {
                if (serializedProperty.GetArrayElementAtIndex(i).isExpanded)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Expands all array elements.
        /// </summary>
        public static void ExpandArray(this SerializedProperty serializedProperty)
        {
            if (!serializedProperty.isArray)
                return;

            int count = serializedProperty.arraySize;
            for (int i = 0; i < count; i++)
            {
                serializedProperty.GetArrayElementAtIndex(i).isExpanded = true;
            }
        }

        /// <summary>
        /// Shrinks all array elements.
        /// </summary>
        public static void CollapseArray(this SerializedProperty serializedProperty)
        {
            if (!serializedProperty.isArray)
                return;

            int count = serializedProperty.arraySize; for (int i = 0; i < count; i++)
            {
                serializedProperty.GetArrayElementAtIndex(i).isExpanded = false;
            }
        }
    }
}