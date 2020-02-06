using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    [CustomPropertyDrawer(typeof(RangedInt))]
    public class RangedIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Cache
            SerializedProperty value = property.FindPropertyRelative("_value");
            SerializedProperty minLimit = property.FindPropertyRelative("minLimit");
            SerializedProperty maxLimit = property.FindPropertyRelative("maxLimit");
            float labelWidth = EditorGUIUtility.labelWidth;
            int fieldWidth = 40;
            Rect labelRect = position.SetWidth(labelWidth);
            Rect sliderRect = new Rect(position.x + labelWidth + fieldWidth + 4, position.y, position.width - labelWidth - fieldWidth * 2 - 6, position.height);
            Rect minRect = position.SetX(position.x + labelWidth);
            minRect.width = fieldWidth;
            Rect maxRect = minRect.SetX(position.x + position.width - fieldWidth);

            // GUI
            GUI.Label(labelRect, property.prefabOverride ? new GUIContent(label) { text = label.text.Bold() } : label);
            minLimit.intValue = EditorGUI.DelayedIntField(minRect, minLimit.intValue);
            value.intValue = EditorGUI.IntSlider(sliderRect, value.intValue, minLimit.intValue, maxLimit.intValue);
            maxLimit.intValue = EditorGUI.DelayedIntField(maxRect, maxLimit.intValue);
        }
    }

    [CustomPropertyDrawer(typeof(RangedFloat))]
    public class RangedFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Cache
            SerializedProperty value = property.FindPropertyRelative("_value");
            SerializedProperty minLimit = property.FindPropertyRelative("minLimit");
            SerializedProperty maxLimit = property.FindPropertyRelative("maxLimit");
            float labelWidth = EditorGUIUtility.labelWidth;
            int fieldWidth = 40;
            Rect labelRect = position.SetWidth(labelWidth);
            Rect sliderRect = new Rect(position.x + labelWidth + fieldWidth + 4, position.y, position.width - labelWidth - fieldWidth * 2 - 6, position.height);
            Rect minRect = position.SetX(position.x + labelWidth);
            minRect.width = fieldWidth;
            Rect maxRect = minRect.SetX(position.x + position.width - fieldWidth);

            // GUI
            GUI.Label(labelRect, property.prefabOverride ? new GUIContent(label) { text = label.text.Bold() } : label);
            minLimit.floatValue = EditorGUI.DelayedFloatField(minRect, minLimit.floatValue);
            value.floatValue = EditorGUI.Slider(sliderRect, value.floatValue, minLimit.floatValue, maxLimit.floatValue);
            maxLimit.floatValue = EditorGUI.DelayedFloatField(maxRect, maxLimit.floatValue);
        }
    }
}
