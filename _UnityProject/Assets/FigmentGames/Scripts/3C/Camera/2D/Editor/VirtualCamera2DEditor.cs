using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    using static EnhancedEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(VirtualCamera2D))]
    public class VirtualCamera2DEditor : Editor
    {
        private VirtualCamera2D virtualCamera;
        private SerializedProperty distanceCalculation;

        public override void OnInspectorGUI()
        {
            // Cache
            virtualCamera = target as VirtualCamera2D;

            // GUI
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_blendCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_priority"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_FOV"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_ratioPreview"));

                distanceCalculation = serializedObject.FindProperty("distanceCalculation");
                EditorGUILayout.PropertyField(distanceCalculation);

                LargeSpace();

                GUILayout.Label("<b>ANCHORS</b>", EnhancedGUI.richText);

                LargeSpace();

                //GUI.enabled = !EditorApplication.isPlaying;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_transformAnchors"), true);
                //GUI.enabled = true;

                SmallSpace();

                GUILayout.BeginHorizontal("box");
                {
                    int count = virtualCamera.transformAnchors.Count;
                    string text = count == 0 ?
                        "<color=#ff4040>At least one transform must be assigned for this virtual camera to work.</color>" :
                        $"Current focus mode: <color={(count == 1 ? "orange" : "#ffffff40")}>Single Anchor</color> - <color={(count > 1 ? "orange" : "#ffffff40")}>Multiple Anchors</color>";
                    GUILayout.Label(text, EnhancedGUI.richLabelWrapStyle);
                }
                GUILayout.EndHorizontal();

                SmallSpace();

                StartCategory(this, "Main Parameters", DrawMainParameters);
                StartCategory(this, "Single Anchor", DrawSingleAnchorParameters);
                StartCategory(this, "Multiple Anchors", DrawMultipleAnchorsParameters);
                StartCategory(this, "Editor Debug", DrawEditorDebug);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawMainParameters()
        {
            switch (distanceCalculation.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultCameraDistance"));
                    break;

                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultFrustumWidth"));
                    break;

                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultFrustumHeight"));
                    break;
            }

            SmallSpace();

            GUILayout.BeginHorizontal();
            {
                SerializedProperty followLerpEnabled = serializedObject.FindProperty("followLerpEnabled");

                GUI.color = Color.white.Alpha(followLerpEnabled.boolValue ? 1f : 0.5f);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_followLerpRate"));

                GUI.color = followLerpEnabled.boolValue ? Color.green : Color.red;
                if (GUILayout.Button(followLerpEnabled.boolValue ? "On" : "Off", GUILayout.Width(32), GUILayout.Height(16)))
                    followLerpEnabled.boolValue = !followLerpEnabled.boolValue;
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();

            SmallSpace();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraFrameColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("barycenterColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPointColor"));

            SmallSpace();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));
        }

        private void DrawSingleAnchorParameters()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorLimits"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dragFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dragFactor"));

            SmallSpace();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("dragFrameColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dragFrameLimitsColor"));
        }

        private void DrawMultipleAnchorsParameters()
        {
            switch (distanceCalculation.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxCameraDistance"));
                    break;

                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxFrustumWidth"));
                    break;

                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxFrustumHeight"));
                    break;
            }

            SmallSpace();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoomLerpRate"));

            SmallSpace();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorsMinFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorsMaxFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorsEdgeOffset"));

            SmallSpace();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("anchorsEdgeOffsetColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("anchorsMinMaxFramesColor"));
        }

        private void DrawEditorDebug()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawGizmosUnselected"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hideAllGizmos"));

            SmallSpace();

            GUI.enabled = EditorApplication.isPlaying && virtualCamera.transformAnchors.Count == 1;

            if (!EditorApplication.isPlaying)
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("The options below ☟ can only be used in play mode.", EnhancedGUI.centeredWrapTextStyle);
                }
                GUILayout.EndVertical();
            }
            else if (virtualCamera.transformAnchors.Count != 1)
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("The options below ☟ can only be used in <color=orange>Single Anchor</color> mode.", EnhancedGUI.centeredWrapTextStyle);
                }
                GUILayout.EndVertical();
            }

            SmallSpace();

            // Reset target point offset
            SerializedProperty resetTargetPointOffset = serializedObject.FindProperty("resetTargetPointOffset");

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(resetTargetPointOffset);
            }
            if (EditorGUI.EndChangeCheck())
            {
                virtualCamera.ResetTargetPointOffset((VirtualCamera2D.SnapAlign)resetTargetPointOffset.enumValueIndex);
            }

            SmallSpace();

            // Reset target point
            GUILayout.BeginHorizontal();
            {
                SerializedProperty resetTargetPoint = serializedObject.FindProperty("resetTargetPoint");
                EditorGUILayout.PropertyField(resetTargetPoint);

                if (GUILayout.Button("Apply", GUILayout.Width(80)))
                {
                    virtualCamera.ResetTargetPoint(resetTargetPoint.vector2Value);
                }
            }
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }
    }
}