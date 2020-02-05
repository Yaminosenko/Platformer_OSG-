using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    [CustomPropertyDrawer(typeof(MinMaxRangeSlider))]
    public class MinMaxRangeSliderDrawer : PropertyDrawer
    {
        MinMaxRangeSlider minMax { get { return (MinMaxRangeSlider)attribute; } }

        private bool IsValid (SerializedProperty property)
        {
            string propertyType = property.type;
            return propertyType == "MinMaxRangeInt" || propertyType == "MinMaxRangeFloat";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float defaultHeight = base.GetPropertyHeight(property, label);
            return IsValid(property) ? defaultHeight * 2 + EditorGUIUtility.standardVerticalSpacing : defaultHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Property is not a MinMaxRangeInt or a MinMaxRangeFloat
            if (!IsValid(property))
            {
                EditorGUI.PropertyField(position, property);
                return;
            }

            // Cache
            bool guiEnabled = GUI.enabled;
            SerializedProperty minValueProperty = property.FindPropertyRelative("_minValue");
            SerializedProperty maxValueProperty = property.FindPropertyRelative("_maxValue");
            bool propertyIsInt = minValueProperty.propertyType == SerializedPropertyType.Integer;
            float minValue = propertyIsInt ? minValueProperty.intValue : minValueProperty.floatValue;
            float maxValue = propertyIsInt ? maxValueProperty.intValue : maxValueProperty.floatValue;
            float minLimit = propertyIsInt ? (int)minMax.minLimit : minMax.minLimit;
            float maxLimit = propertyIsInt ? (int)minMax.maxLimit : minMax.maxLimit;

            float labelWidth = EditorGUIUtility.labelWidth;
            float defaultLineHeight = position.height / 2f;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float fieldSize = (position.width - labelWidth) / 4f;
            Rect labelRect = position.SetWidth(labelWidth);
            Rect sliderRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, defaultLineHeight);
            Rect minLimitRect = new Rect(position.x + labelWidth, position.y + defaultLineHeight + spacing, fieldSize, defaultLineHeight);
            Rect minValueRect = minLimitRect.XOffset(fieldSize);
            Rect maxValueRect = minValueRect.XOffset(fieldSize);
            Rect maxLimitRect = maxValueRect.XOffset(fieldSize);

            // GUI
            GUI.Label(labelRect, label, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft });

            EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref minValue, ref maxValue, minLimit, maxLimit);

            GUI.enabled = false;
            EditorGUI.FloatField(minLimitRect, minLimit);
            GUI.enabled = guiEnabled;

            float minValueEdit = propertyIsInt ? EditorGUI.DelayedIntField(minValueRect, (int)minValue) : EditorGUI.DelayedFloatField(minValueRect, minValue);
            minValueEdit = propertyIsInt ? EnhancedMath.IntClamp((int)minValueEdit, (int)minLimit, (int)maxValue) : Mathf.Clamp(minValueEdit, minLimit, maxValue);
            minValue = minValueEdit;

            float maxValueEdit = propertyIsInt ? EditorGUI.DelayedIntField(maxValueRect, (int)maxValue) : EditorGUI.DelayedFloatField(maxValueRect, maxValue);
            maxValueEdit = propertyIsInt ? EnhancedMath.IntClamp((int)maxValueEdit, (int)minValue, (int)maxLimit) : Mathf.Clamp(maxValueEdit, minValue, maxLimit);
            maxValue = maxValueEdit;

            GUI.enabled = false;
            EditorGUI.FloatField(maxLimitRect, maxLimit);
            GUI.enabled = guiEnabled;

            // Apply modifications
            if (propertyIsInt)
            {
                minValueProperty.intValue = Mathf.RoundToInt(minValue);
                maxValueProperty.intValue = Mathf.RoundToInt(maxValue);
            }
            else
            {
                minValueProperty.floatValue = minValue;
                maxValueProperty.floatValue = maxValue;
            }
        }
    }
}
