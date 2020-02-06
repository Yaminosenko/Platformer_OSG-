#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    using static EnhancedMath;
    using static EnhancedGizmos;

    public partial class VirtualCamera2D : VirtualCamera
    {
        //[Space(10)]
        //[Header("EDITOR")]
        [SerializeField] private SnapAlign resetTargetPointOffset;
        [SerializeField] private Vector2 resetTargetPoint;

        //[Space(10)]
        //[Header("GIZMOS")]
        [SerializeField] private bool drawGizmosUnselected = true;
        [SerializeField] private bool hideAllGizmos = false;

        [HideInInspector] [SerializeField] private bool drawCameraFrustum = true;
        [SerializeField] [OnOff("drawCameraFrustum", "Camera Frame")] private Color cameraFrameColor = Color.yellow;

        [HideInInspector] [SerializeField] private bool drawBarycenter = true;
        [SerializeField] [OnOff("drawBarycenter", "Barycenter")] private Color barycenterColor = Color.red;

        [HideInInspector] [SerializeField] private bool drawTargetPoint = true;
        [SerializeField] [OnOff("drawTargetPoint", "Target Point")] private Color targetPointColor = Color.cyan;

        [SerializeField] private bool displayName = true;

        [HideInInspector] [SerializeField] private bool drawDragFrame = true;
        [SerializeField] [OnOff("drawDragFrame", "Frame")] private Color dragFrameColor = Color.green;

        [HideInInspector] [SerializeField] private bool drawDragFrameLimits = true;
        [SerializeField] [OnOff("drawDragFrameLimits", "Limits")] private Color dragFrameLimitsColor = Color.cyan;

        [HideInInspector] [SerializeField] private bool drawAnchorsEdgeOffset = false;
        [SerializeField] [OnOff("drawAnchorsEdgeOffset", "Edge Offset")] private Color anchorsEdgeOffsetColor = Color.red;

        [HideInInspector] [SerializeField] private bool drawAnchorsMinMaxFrames = false;
        [SerializeField] [OnOff("drawAnchorsMinMaxFrames", "Min Max Frames")] private Color anchorsMinMaxFramesColor = Color.magenta;

        // Cache
        [HideInInspector] public float previousDefaultCameraDistance;
        [HideInInspector] public float previousDefaultFrustumWidth;
        [HideInInspector] public float previousDefaultFrustumHeight;
        [HideInInspector] public float previousMaxCameraDistance;
        [HideInInspector] public float previousMaxFrustumWidth;
        [HideInInspector] public float previousMaxFrustumHeight;

        #region UNITY

        protected override void OnValidate()
        {
            base.OnValidate();

            // Hard set
            anchorsEdgeOffset = anchorsEdgeOffset;
            anchorLimits = anchorLimits;
            dragFrame = dragFrame;

            // Camera distance
            if (!soloMode && previousDefaultCameraDistance != defaultCameraDistance)
            {
                defaultCameraDistance = defaultCameraDistance;
            }
            previousDefaultCameraDistance = defaultCameraDistance;

            if (soloMode || previousMaxCameraDistance != maxCameraDistance)
            {
                maxCameraDistance = maxCameraDistance;
            }
            previousMaxCameraDistance = maxCameraDistance;

            // Frustum width
            if (!soloMode && previousDefaultFrustumWidth != defaultFrustumWidth)
            {
                defaultFrustumWidth = defaultFrustumWidth;
            }
            previousDefaultFrustumWidth = defaultFrustumWidth;

            if (soloMode || previousMaxFrustumWidth != maxFrustumWidth)
            {
                maxFrustumWidth = maxFrustumWidth;
            }
            previousMaxFrustumWidth = maxFrustumWidth;

            // Frustum height
            if (!soloMode && previousDefaultFrustumHeight != defaultFrustumHeight)
            {
                defaultFrustumHeight = defaultFrustumHeight;
            }
            previousDefaultFrustumHeight = defaultFrustumHeight;

            if (soloMode || previousMaxFrustumHeight != maxFrustumHeight)
            {
                maxFrustumHeight = maxFrustumHeight;
            }
            previousMaxFrustumHeight = maxFrustumHeight;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (drawGizmosUnselected)
                DrawCameraGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmosUnselected)
                DrawCameraGizmos();
        }

        #endregion

        #region GIZMOS

        protected override void EditorUpdate()
        {
            base.EditorUpdate();

            if (EditorApplication.isPlaying)
                return;

            // Lock transform rotation and scale
            rotation = Quaternion.identity;
            localScale = Vector3.one;

            // Position (XY)
            GetAnchorsSpacing();
            GetBarycenterPosition();
            currentTargetPoint = targetPoint = barycenter;
            targetPointOffset = Vector2.zero;

            // Zoom (Z)
            GetCameraDistance();

            // Apply
            ApplyTargetPosition();
        }


        private void DrawCameraGizmos()
        {
            if (hideAllGizmos)
                return;

            DrawAnchorsEdgeOffset();
            DrawDragFrameLimits();
            DrawDragFrame();
            DrawTargetPoint();
            DrawBarycenter();
            DrawCameraFrustum();
            DrawAnchorsMinMaxFrames();
        }

        private void DrawAnchorsEdgeOffset()
        {
            if (!drawAnchorsEdgeOffset)
                return;

            // Cache
            Vector3[] maxCamDistanceFrustumCorners = GetFrustumCorners(position, rotation, FOV, aspect, currentCameraDistance);
            Vector3[] anchorsEdgeOffsetFrameCorners = GetRectCorners(position, rotation, anchorsEdgeOffsetFrame);

            // Draw
            DrawRect(anchorsEdgeOffsetColor, position, rotation, anchorsEdgeOffsetFrame);

            if (!drawCameraFrustum)
                return;

            Handles.color = anchorsEdgeOffsetColor.Alpha(0.25f);
            Handles.DrawDottedLine(anchorsEdgeOffsetFrameCorners[0], maxCamDistanceFrustumCorners[0], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(anchorsEdgeOffsetFrameCorners[1], maxCamDistanceFrustumCorners[1], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(anchorsEdgeOffsetFrameCorners[2], maxCamDistanceFrustumCorners[2], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(anchorsEdgeOffsetFrameCorners[3], maxCamDistanceFrustumCorners[3], thinDottedScreenSpaceSize);
        }

        private void DrawTargetPoint()
        {
            if (!drawTargetPoint)
                return;

            // Target point
            Handles.color = targetPointColor;
            Handles.SphereHandleCap(-1, targetPoint, Quaternion.identity, 0.5f, EventType.Repaint);

            // Dotted line
            Handles.color = targetPointColor.AlphaRelative(0.5f); ;
            Handles.DrawDottedLine(position.ZValue(0), targetPoint, defaultDottedScreenSpaceSize);
        }

        private void DrawDragFrameLimits()
        {
            if (!drawDragFrameLimits)
                return;

            // Cache
            Vector3[] dragFrameLimitsCorners = GetRectCorners(position, rotation, anchorWorldLimits);

            // Drag frame world limits
            Handles.color = dragFrameLimitsColor;
            DrawRect(position, rotation, anchorWorldLimits);

            // Outer dotted lines
            if (!drawCameraFrustum)
                return;

            Handles.color = dragFrameLimitsColor.AlphaRelative(0.2f);
            Vector2 frameSpacing = (targetFrustum - anchorWorldLimits) * 0.5f;

            Handles.DrawDottedLine(dragFrameLimitsCorners[0], dragFrameLimitsCorners[0] + new Vector3(0, -frameSpacing.y), defaultDottedScreenSpaceSize);
            Handles.DrawDottedLine(dragFrameLimitsCorners[0], dragFrameLimitsCorners[0] + new Vector3(-frameSpacing.x, 0), defaultDottedScreenSpaceSize);

            Handles.DrawDottedLine(dragFrameLimitsCorners[1], dragFrameLimitsCorners[1] + new Vector3(0, frameSpacing.y), defaultDottedScreenSpaceSize);
            Handles.DrawDottedLine(dragFrameLimitsCorners[1], dragFrameLimitsCorners[1] + new Vector3(-frameSpacing.x, 0), defaultDottedScreenSpaceSize);

            Handles.DrawDottedLine(dragFrameLimitsCorners[2], dragFrameLimitsCorners[2] + new Vector3(0, frameSpacing.y), defaultDottedScreenSpaceSize);
            Handles.DrawDottedLine(dragFrameLimitsCorners[2], dragFrameLimitsCorners[2] + new Vector3(frameSpacing.x, 0), defaultDottedScreenSpaceSize);

            Handles.DrawDottedLine(dragFrameLimitsCorners[3], dragFrameLimitsCorners[3] + new Vector3(0, -frameSpacing.y), defaultDottedScreenSpaceSize);
            Handles.DrawDottedLine(dragFrameLimitsCorners[3], dragFrameLimitsCorners[3] + new Vector3(frameSpacing.x, 0), defaultDottedScreenSpaceSize);
        }

        private void DrawDragFrame()
        {
            if (!drawDragFrame)
                return;

            DrawRect(dragFrameColor, targetPoint - targetPointOffset, rotation, dragFrame);
        }

        private void DrawBarycenter()
        {
            if (!drawBarycenter)
                return;

            // Barycenter dot
            Handles.color = barycenterColor;
            Handles.SphereHandleCap(-1, barycenter, Quaternion.identity, 0.375f, EventType.Repaint);

            if (soloMode)
                return;

            for (int i = 0; i < transformAnchors.Count; i++)
            {
                // Draw dotted line from transform anchor to barycenter
                if (transformAnchors[i])
                {
                    // Cache
                    Vector2 pos = transformAnchors[i].position;
                    Vector2 clampedPos = FramePoint(pos, position, anchorsEdgeOffsetFrame);

                    // Out of frame line
                    if (clampedPos != pos)
                    {
                        Handles.color = Color.gray.Alpha(barycenterColor.a * 0.25f);
                        Handles.DrawDottedLine(clampedPos, pos, defaultDottedScreenSpaceSize);

                        Handles.color = Color.gray.Alpha(barycenterColor.a * 0.5f);
                        Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.375f, EventType.Repaint);
                    }

                    // Main line
                    Handles.color = barycenterColor.Alpha(0.5f);
                    Handles.DrawDottedLine(barycenter, clampedPos, defaultDottedScreenSpaceSize);

                    Handles.color = barycenterColor;
                    Handles.SphereHandleCap(-1, clampedPos, Quaternion.identity, 0.375f, EventType.Repaint);
                }
            }
        }

        private void DrawCameraFrustum()
        {
            if (!drawCameraFrustum)
                return;

            // Current frustum
            Handles.color = cameraFrameColor;

            DrawRect(position, rotation, targetFrustum);

            Handles.SphereHandleCap(-1, (Vector2)position, Quaternion.identity, 0.25f, EventType.Repaint);

            // Label
            if (displayName)
            {
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
                EnhancedGizmos.Label($"{gameObject.name} {textMode}", (Vector2)position - targetFrustum * 0.5f, cameraFrameColor, 40, 5);
            }

            // Min - Max frustums
            Vector3[] defaultCamDistanceFrustumCorners = GetFrustumCorners(position, rotation, FOV, aspect, defaultDistance);
            Vector3[] maxCamDistanceFrustumCorners = GetFrustumCorners(position, rotation, FOV, aspect, maxDistance);

            Handles.color = (soloMode ? Color.gray : cameraFrameColor).Alpha(cameraFrameColor.a * 0.25f);

            DrawFrustumRect(position, rotation, FOV, aspect, defaultDistance);
            DrawFrustumRect(position, rotation, FOV, aspect, maxDistance);

            Handles.DrawDottedLine(defaultCamDistanceFrustumCorners[0], maxCamDistanceFrustumCorners[0], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(defaultCamDistanceFrustumCorners[1], maxCamDistanceFrustumCorners[1], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(defaultCamDistanceFrustumCorners[2], maxCamDistanceFrustumCorners[2], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(defaultCamDistanceFrustumCorners[3], maxCamDistanceFrustumCorners[3], thinDottedScreenSpaceSize);
        }

        private void DrawAnchorsMinMaxFrames()
        {
            if (!drawAnchorsMinMaxFrames)
                return;

            // Cache
            Vector3[] minAnchorsFrameCorners = GetRectCorners(barycenter, rotation, anchorsMinFrame);
            Vector3[] maxAnchorsFrameCorners = GetRectCorners(barycenter, rotation, anchorsMaxFrame);

            // Current anchors frame
            Handles.color = anchorsMinMaxFramesColor;

            // Anchors spacing frame (lerped)
            Handles.color = anchorsMinMaxFramesColor;

            DrawRect(barycenter, rotation, Vector2.Lerp(anchorsMinFrame, anchorsMaxFrame, anchorsSpacingLerp));

            // Min - Max frames
            Handles.color = anchorsMinMaxFramesColor.Alpha(anchorsMinMaxFramesColor.a * 0.25f);

            DrawRect(barycenter, rotation, anchorsMinFrame);
            DrawRect(barycenter, rotation, anchorsMaxFrame);

            Handles.DrawDottedLine(minAnchorsFrameCorners[0], maxAnchorsFrameCorners[0], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(minAnchorsFrameCorners[1], maxAnchorsFrameCorners[1], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(minAnchorsFrameCorners[2], maxAnchorsFrameCorners[2], thinDottedScreenSpaceSize);
            Handles.DrawDottedLine(minAnchorsFrameCorners[3], maxAnchorsFrameCorners[3], thinDottedScreenSpaceSize);
        }

        #endregion

        [MenuItem("GameObject/Figment Games/Camera/Virtual Camera 2D")]
        static void CreateVirtualCamera2D(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("VirtualCamera2D");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add the component
            go.AddComponent<VirtualCamera2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif