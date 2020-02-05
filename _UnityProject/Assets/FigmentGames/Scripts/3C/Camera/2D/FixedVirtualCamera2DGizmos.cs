#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    using static EnhancedGizmos;

    public partial class FixedVirtualCamera2D : VirtualCamera
    {
        [Space(10)]
        [Header("GIZMOS")]
        public bool drawGizmosUnselected = true;
        [Space]
        [SerializeField] [OnOff("drawCameraFrustum", "Camera Frame")] private Color _cameraFrameColor = Color.yellow.Alpha(0.5f);
        public Color cameraFrameColor { get { return _cameraFrameColor; } }
        [HideInInspector, SerializeField] private bool drawCameraFrustum = true;
        [SerializeField] private bool displayName = true;


        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (drawGizmosUnselected)
                DrawCameraFrustum();
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmosUnselected)
                DrawCameraFrustum();
        }


        private void DrawCameraFrustum()
        {
            if (!drawCameraFrustum)
                return;

            // Cache
            Vector2 frustum = Vector2.zero;

            switch(distanceCalculation)
            {
                case DistanceCalculation.Simple:
                    frustum = EnhancedMath.GetScreenFrustumAtDistance(FOV, cameraDistance);
                    break;

                case DistanceCalculation.FrustumWidth:
                    frustum = new Vector2(frustumWidth, frustumWidth * ratioPreview.y / ratioPreview.x);
                    break;

                case DistanceCalculation.FrustumHeight:
                    frustum = new Vector2(frustumHeight * ratioPreview.x / ratioPreview.y, frustumHeight);
                    break;
            }

            // Label
            if (displayName)
            {
                Vector3[] corners = EnhancedMath.GetRectCorners(position.ZValue(0), Quaternion.identity, frustum);
                string textMode = "";
                switch (distanceCalculation)
                {
                    case DistanceCalculation.FrustumWidth:
                        textMode = "↔";
                        break;

                    case DistanceCalculation.FrustumHeight:
                        textMode = "↕";
                        break;
                }
                EnhancedGizmos.Label($"{gameObject.name} {textMode}", corners[0], cameraFrameColor, 40, 5);
            }

            // Draw
            Handles.color = cameraFrameColor;
            DrawRect(position, rotation, frustum, true);
        }


        [MenuItem("GameObject/Figment Games/Camera/Fixed Virtual Camera 2D")]
        static void CreateFixedVirtualCamera2D(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("FixedVirtualCamera2D");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add the component
            go.AddComponent<FixedVirtualCamera2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif