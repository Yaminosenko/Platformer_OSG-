using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FixedVirtualCamera2D))]
    public class FixedVirtualCamera2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_blendCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_priority"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_FOV"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_ratioPreview"));

            SerializedProperty distanceCalculation = serializedObject.FindProperty("distanceCalculation");
            EditorGUILayout.PropertyField(distanceCalculation);

            switch(distanceCalculation.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraDistance"));
                    break;

                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("frustumWidth"));
                    break;

                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("frustumHeight"));
                    break;
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawGizmosUnselected"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_cameraFrameColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}