#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    using static EnhancedMath;
    using static EnhancedGizmos;

    [RequireComponent(typeof(Camera))]
    public partial class CameraController2D : EnhancedMonoBehaviour
    {
        [Space(10)]
        [Header("EDITOR")]
        [Tooltip("A virtual camera can be set here to preview its behaviour out of play mode.")]
        [SerializeField] private VirtualCamera debugVirtualCamera;
        [Tooltip("A constraint can be set here to preview its behaviour out of play mode.")]
        [SerializeField] private CameraConstraint2D debugConstraint;

        [Space(10)]
        [Header("GIZMOS")]
        [SerializeField] private bool drawGizmosUnselected = true;

        [HideInInspector] [SerializeField] private bool drawCameraFrustum = true;
        [Space]
        [SerializeField] [OnOff("drawCameraFrustum", "Color")] private Color cameraFrameColor = Color.white;

        #region UNITY

        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            // Next code executes kind of in Awake

            debugVirtualCamera = null;
            debugConstraint = null;
        }

        private void OnDrawGizmos()
        {
            EditorUpdate();

            if (drawGizmosUnselected)
                DrawCameraControllerGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmosUnselected)
                DrawCameraControllerGizmos();
        }

        #endregion

        #region GIZMOS

        private void EditorUpdateTargetPoints()
        {
            virtualTargetPoint = debugVirtualCamera ? debugVirtualCamera.GetControllerAnchor(this) : position;

            constraintTargetPoint = debugConstraint ? debugConstraint.GetConstraintPosition(this) : virtualTargetPoint;
        }

        private void EditorUpdate()
        {
            if (EditorApplication.isPlaying)
                return;

            rotation = Quaternion.identity;
            localScale = Vector3.one;

            EditorUpdateTargetPoints();
            SnapToTargetPoint();

            if (debugVirtualCamera)
                camera.fieldOfView = debugVirtualCamera.FOV;
        }

        private void DrawCameraControllerGizmos()
        {
            if (!drawCameraFrustum)
                return;

            // Cache
            Vector2 frustum = GetCameraFrustumAtDistance(camera, Mathf.Abs(position.z));
            bool highlight = (EditorApplication.isPlaying && currentVirtualCamera) || (!EditorApplication.isPlaying && debugVirtualCamera);
            float timeCos = Mathf.Cos((float)EditorApplication.timeSinceStartup * 8f) * 0.5f + 0.5f;
            Vector3[] corners = GetRectCorners(position.ZValue(0), rotation, frustum);

            // Gizmos
            Color color = position.z > 0f ? Color.red.Alpha(timeCos) : cameraFrameColor;
            Handles.color = color;

            DrawRect(color, position.ZValue(0), rotation, frustum, highlight ? defaultLineWidth + timeCos * 2 : defaultLineWidth);

            Handles.color = color.AlphaRelative(0.25f);
            Handles.DrawDottedLine(position, corners[0], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(position, corners[1], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(position, corners[2], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(position, corners[3], thinDottedScreenSpaceSize);

            string prefix = "";
            if ((!EditorApplication.isPlaying && debugVirtualCamera) ||
                (EditorApplication.isPlaying && currentVirtualCamera))
                prefix = "❑ ";
            float offset = HandleUtility.GetHandleSize(corners[1]) * 0.18f;
            EnhancedGizmos.Label($"{prefix}{gameObject.name}", corners[1] + Vector3.up * offset, cameraFrameColor, 40, 5);
        }

        #endregion

        [MenuItem("GameObject/Figment Games/Camera/Camera Controller 2D")]
        static void CreateCameraController2D(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraController2D");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add the component
            go.AddComponent<CameraController2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif