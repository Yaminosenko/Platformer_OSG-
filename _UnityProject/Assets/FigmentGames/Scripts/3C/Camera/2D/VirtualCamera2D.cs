using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FigmentGames
{
    public partial class VirtualCamera2D : VirtualCamera
    {
        // Main parameters
        [Tooltip(
            "The list of all the transforms this virtual camera has to follow. The focus mode depends on how many transforms have been assigned." +
            "\n\nN.B: At runtime, transforms should be set and removed with the CameraController2D methods AddTransformAnchor and RemoveTransformAnchor.")]
        [SerializeField] private List<Transform> _transformAnchors = new List<Transform>();
        public List<Transform> transformAnchors { get { return _transformAnchors; } }

        [Tooltip("The closest distance of the camera, regardless of its focus mode.")]
        [SerializeField] [MinValue(0)] private float _defaultCameraDistance = 24f;
        private float defaultCameraDistance
        {
            get
            {
                return _defaultCameraDistance;
            }
            set
            {
                if (value > maxCameraDistance)
                    value = maxCameraDistance;

                _defaultCameraDistance = value;
            }
        }
        [Tooltip("The smallest frustum width the camera needs to fit to, regardless of its focus mode.")]
        [SerializeField] [MinValue(0)] private float _defaultFrustumWidth = 32f;
        private float defaultFrustumWidth
        {
            get
            {
                return _defaultFrustumWidth;
            }
            set
            {
                if (value > maxFrustumWidth)
                    value = maxFrustumWidth;

                _defaultFrustumWidth = value;
            }
        }
        [Tooltip("The smallest frustum height the camera needs to fit to, regardless of its focus mode.")]
        [SerializeField] [MinValue(0)] private float _defaultFrustumHeight = 18f;
        private float defaultFrustumHeight
        {
            get
            {
                return _defaultFrustumHeight;
            }
            set
            {
                if (value > maxFrustumHeight)
                    value = maxFrustumHeight;

                _defaultFrustumHeight = value;
            }
        }
        //[Space]
        [Tooltip("This value determines how smoothly the camera follows the transform anchors. The lowest this value is, the smoother is the movement.")]
        [SerializeField] [MinValue(0)] private float _followLerpRate = 8f;
        private float followLerpRate { get { return _followLerpRate; } set { if (value < 0f) value = 0f; _followLerpRate = value; } }
        [SerializeField, HideInInspector] private bool followLerpEnabled = true;

        // Single anchor
        [Tooltip("The biggest normalized rectangle the followed transform anchor can fit in. The X and Y values represent the normalized part of the current camera frustum.")]
        [SerializeField] private Vector2 _anchorLimits = new Vector2(0.333f, 0.333f);
        public Vector2 anchorLimits { get { return _anchorLimits; } private set { _anchorLimits = value.Clamp(Vector2.zero, Vector2.one); } }
        [Tooltip("The size of the drag frame in world space.")]
        [SerializeField] private Vector2 _dragFrame = new Vector2(2, 4);
        private Vector2 dragFrame { get { return _dragFrame; } set { _dragFrame = value.Min(0, 0); } }
        [Tooltip("The higher this value is, the stronger is offseted the camera when the drag frame is moved.")]
        [SerializeField] [MinValue(0)] private float dragFactor = 1f;

        // Multiple anchors
        [Tooltip("The farthest distance of the camera, regardless of its focus mode.")]
        [SerializeField] private float _maxCameraDistance = 32f;
        private float maxCameraDistance
        {
            get
            {
                return _maxCameraDistance;
            }
            set
            {
                if (value < defaultCameraDistance)
                    value = defaultCameraDistance;

                _maxCameraDistance = value;
            }
        }
        [Tooltip("The biggest frustum width the camera needs to fit to, regardless of its focus mode.")]
        [SerializeField] private float _maxFrustumWidth = 42f;
        private float maxFrustumWidth
        {
            get
            {
                return _maxFrustumWidth;
            }
            set
            {
                if (value < defaultFrustumWidth)
                    value = defaultFrustumWidth;

                _maxFrustumWidth = value;
            }
        }
        [Tooltip("The biggest frustum width the camera needs to fit to, regardless of its focus mode.")]
        [SerializeField] private float _maxFrustumHeight = 24f;
        private float maxFrustumHeight
        {
            get
            {
                return _maxFrustumHeight;
            }
            set
            {
                if (value < defaultFrustumHeight)
                    value = defaultFrustumHeight;

                _maxFrustumHeight = value;
            }
        }
        [Space]
        [Tooltip("This value determines how smoothly the camera changes its zoom (distance) when following the transform anchors. The lowest this value is, the smoother is the movement.")]
        [SerializeField] [MinValue(0)] private float zoomLerpRate = 8f;
        [Space]
        [Tooltip("When the overall spacing of all the transform anchors fit in this rectangle, the camera is at its minimum distance.")]
        [SerializeField] private Vector2 _anchorsMinFrame = new Vector2(16f, 9f);
        public Vector2 anchorsMinFrame
        {
            get
            {
                return _anchorsMinFrame;
            }
            private set
            {
                // X
                if (value.x < 0f)
                    value.x = 0f;

                if (value.x > anchorsMaxFrame.x)
                    value.x = anchorsMaxFrame.x;

                // Y
                if (value.y < 0f)
                    value.y = 0f;

                if (value.y > anchorsMaxFrame.y)
                    value.y = anchorsMaxFrame.y;

                // Apply
                _anchorsMinFrame = value;
            }
        }
        [Tooltip("When the overall spacing of all the transform anchors exceeds this rectangle, the camera is at its maximum distance.")]
        [SerializeField] private Vector2 _anchorsMaxFrame = new Vector2(32f, 18f);
        public Vector2 anchorsMaxFrame
        {
            get
            {
                return _anchorsMaxFrame;
            }
            private set
            {
                if (value.x < anchorsMinFrame.x)
                    value.x = anchorsMinFrame.x;

                if (value.y < anchorsMinFrame.y)
                    value.y = anchorsMinFrame.y;

                _anchorsMaxFrame = value;
            }
        }
        [Space]
        [SerializeField] private Vector2 _anchorsEdgeOffset = new Vector2(2, 2);
        public Vector2 anchorsEdgeOffset { get { return _anchorsEdgeOffset; } private set { _anchorsEdgeOffset = value.Min(0, 0); } }
        

        // Enums
        public enum SnapAlign
        {
            Center,
            Vertical,
            Horizontal,
            Left,
            Right,
            Top,
            Bottom,
            BottomLeft,
            MiddleLeft,
            TopLeft,
            MiddleTop,
            TopRight,
            MiddleRight,
            BottomRight,
            MiddleBottom
        }

        // Accessors
        public bool hasTransformAnchors { get { return transformAnchors.Count > 0; } }
        public bool soloMode { get { return transformAnchors.Count < 2; } }
        public Vector2 targetFrustum { get { return EnhancedMath.GetFrustumAtDistance(FOV, aspect, currentCameraDistance); } } // Current frustrum size
        public Vector2 anchorsEdgeOffsetFrame { get { return AnchorsEdgeFrame(targetFrustum); } } // Size of the inner anchors edge offset frame
        public Vector2 anchorWorldLimits { get { return targetFrustum * anchorLimits; } }
        private float anchorsSpacingLerp;

        // Cache
        private float deltaTime;
        public Vector2 barycenter { get; private set; }
        private Vector2 targetPoint;
        private Vector2 targetPointOffset;
        private Vector2 currentTargetPoint;
        [SerializeField, HideInInspector] private float currentCameraDistance;
        private float defaultDistance;
        private float maxDistance;
        private float aspect;
        [SerializeField, HideInInspector] private Vector2 anchorsMinPos;
        [SerializeField, HideInInspector] private Vector2 anchorsMaxPos;
        private CameraController2D currentCameraController;


        #region UNITY

        private void Awake()
        {
            // Remove empty elements from transform anchors list
            _transformAnchors = transformAnchors.Where(item => item != null).ToList();

            // Cache & hard set
            GetBarycenterPosition();
            position = currentTargetPoint = targetPoint = barycenter;
            targetPointOffset = Vector2.zero;

            GetCameraDistance();
        }

        private void LateUpdate()
        {
            // Cache
            deltaTime = Time.deltaTime;
            GetAnchorsSpacing();

            // Position (XY)
            GetBarycenterPosition();
            DragTargetPoint();
            LerpCurrentTargetPoint();

            // Zoom (Z)
            GetCameraDistance();

            // Apply
            ApplyTargetPosition();
        }

        #endregion

        #region BEHAVIOUR

        private void Cache()
        {

        }


        protected override void VirtualCameraSet(CameraController2D controller)
        {
            this.currentCameraController = controller;
        }

        protected override void VirtualCameraRemoved()
        {
            this.currentCameraController = null;
        }


        public void AddTransformAnchor(Transform transform)
        {
            // Transform parameter cannot be null
            if (!transform)
                return;

            transformAnchors.Add(transform);
        }

        public void RemoveTransformAnchor(Transform transform)
        {
            // Transform parameter cannot be null
            if (!transform)
                return;

            transformAnchors.Remove(transform);
        }


        private void GetBarycenterPosition()
        {
            if (!hasTransformAnchors)
            {
                barycenter = position;
                return;
            }

            // Solo
            if (soloMode)
            {
                barycenter = transformAnchors[0] ? transformAnchors[0].position : position;
                return;
            }

            // Calculate barycenter position
            barycenter = Vector2.zero;

            int iterations = 0;
            for (int i = 0; i < transformAnchors.Count; i++)
            {
                if (transformAnchors[i])
                {
                    barycenter += FramePoint(transformAnchors[i].position, position, anchorsEdgeOffsetFrame);

                    iterations++;
                }
            }

            // No transform has been found
            if (iterations == 0)
            {
                barycenter = position;
                return;
            }

            barycenter /= iterations;
        }


        private void DragTargetPoint()
        {
            if (!soloMode || !hasTransformAnchors)
            {
                targetPointOffset = Vector2.zero;
                targetPoint = barycenter;
            }
            else
            {
                Vector2 center = targetPoint - targetPointOffset;
                Vector2 offsetVector = barycenter - FramePoint(barycenter, center, dragFrame);

                // Drag
                targetPointOffset += offsetVector * dragFactor;
                targetPoint += offsetVector * (1 + dragFactor);
            }
        }

        private void LerpCurrentTargetPoint()
        {
            // Lerp towards target point
            currentTargetPoint = Vector2.Lerp(currentTargetPoint, targetPoint, followLerpEnabled ? followLerpRate * deltaTime : 1f);

            // Cache
            Vector2 clampedPos = Vector2.zero;

            if (soloMode) // Post lerp clamp position, based on barycenter
            {
                clampedPos = new Vector2(
                    Mathf.Clamp(currentTargetPoint.x, barycenter.x - anchorWorldLimits.x * 0.5f, barycenter.x + anchorWorldLimits.x * 0.5f),
                    Mathf.Clamp(currentTargetPoint.y, barycenter.y - anchorWorldLimits.y * 0.5f, barycenter.y + anchorWorldLimits.y * 0.5f));
            }
            else // Clamp based on min - max anchors position (in case all anchors are outside of their frame)
            {
                Vector2 frame = AnchorsEdgeFrame(EnhancedMath.GetScreenFrustumAtDistance(FOV, currentCameraDistance)) * 0.5f;
                clampedPos = new Vector2(
                    Mathf.Clamp(currentTargetPoint.x, anchorsMinPos.x - frame.x, anchorsMaxPos.x + frame.x),
                    Mathf.Clamp(currentTargetPoint.y, anchorsMinPos.y - frame.y, anchorsMaxPos.y + frame.y));
            }

            // Overwrite values
            Vector2 positionOffset = currentTargetPoint - clampedPos;

            currentTargetPoint = clampedPos;
            targetPoint -= positionOffset;
            targetPointOffset -= positionOffset;
        }

        public void ResetTargetPointOffset(SnapAlign boxAnchor)
        {
            // Vector from drag frame center to barycenter
            Vector2 transformAnchorOffset = barycenter - (targetPoint - targetPointOffset);

            // Re-center the targetPointOffset
            targetPointOffset -= transformAnchorOffset;

            // Recalculate offset
            Vector2 halfDragFrame = dragFrame * 0.5f;
            switch (boxAnchor)
            {
                case SnapAlign.Vertical:
                    targetPointOffset += new Vector2(0, transformAnchorOffset.y);
                    break;

                case SnapAlign.Horizontal:
                    targetPointOffset += new Vector2(transformAnchorOffset.x, 0);
                    break;


                case SnapAlign.Left:
                    targetPointOffset += new Vector2(-halfDragFrame.x, transformAnchorOffset.y);
                    break;

                case SnapAlign.Right:
                    targetPointOffset += new Vector2(halfDragFrame.x, transformAnchorOffset.y);
                    break;

                case SnapAlign.Top:
                    targetPointOffset += new Vector2(transformAnchorOffset.x, halfDragFrame.y);
                    break;

                case SnapAlign.Bottom:
                    targetPointOffset += new Vector2(transformAnchorOffset.x, -halfDragFrame.y);
                    break;


                case SnapAlign.BottomLeft:
                    targetPointOffset -= halfDragFrame;
                    break;

                case SnapAlign.MiddleLeft:
                    targetPointOffset += new Vector2(-halfDragFrame.x, 0);
                    break;

                case SnapAlign.TopLeft:
                    targetPointOffset += new Vector2(-halfDragFrame.x, halfDragFrame.y);
                    break;

                case SnapAlign.MiddleTop:
                    targetPointOffset += new Vector2(0, halfDragFrame.y);
                    break;

                case SnapAlign.TopRight:
                    targetPointOffset += halfDragFrame;
                    break;

                case SnapAlign.MiddleRight:
                    targetPointOffset += new Vector2(halfDragFrame.x, 0);
                    break;

                case SnapAlign.BottomRight:
                    targetPointOffset += new Vector2(halfDragFrame.x, -halfDragFrame.y);
                    break;

                case SnapAlign.MiddleBottom:
                    targetPointOffset += new Vector2(0, -halfDragFrame.y);
                    break;
            }
        }

        public void ResetTargetPoint(Vector2 offset = default)
        {
            Vector2 barycenterOffset = barycenter - targetPoint + offset;

            targetPoint += barycenterOffset;
            targetPointOffset += barycenterOffset;
        }

        private void GetAnchorsSpacing()
        {
            if (soloMode)
                return;

            if (!transformAnchors[0])
                return;

            Vector2 anchorsSpacing;

            anchorsMinPos = transformAnchors[0].position;
            anchorsMaxPos = anchorsMinPos;

            for (int i = 1; i < transformAnchors.Count; i++)
            {
                // Missing transform
                if (!transformAnchors[i])
                    continue;

                Vector2 anchorPos = transformAnchors[i].position;

                // X
                if (anchorPos.x < anchorsMinPos.x)
                    anchorsMinPos.x = anchorPos.x;

                if (anchorPos.x > anchorsMaxPos.x)
                    anchorsMaxPos.x = anchorPos.x;

                // Y
                if (anchorPos.y < anchorsMinPos.y)
                    anchorsMinPos.y = anchorPos.y;

                if (anchorPos.y > anchorsMaxPos.y)
                    anchorsMaxPos.y = anchorPos.y;
            }

            anchorsSpacing = anchorsMaxPos - anchorsMinPos;

            // Clamp
            anchorsSpacing.x = Mathf.Clamp(anchorsSpacing.x, anchorsMinFrame.x, anchorsMaxFrame.x);
            anchorsSpacing.y = Mathf.Clamp(anchorsSpacing.y, anchorsMinFrame.y, anchorsMaxFrame.y);

            // Get in-between frames lerp
            float xLerp = Mathf.InverseLerp(anchorsMinFrame.x, anchorsMaxFrame.x, anchorsSpacing.x);
            float yLerp = Mathf.InverseLerp(anchorsMinFrame.y, anchorsMaxFrame.y, anchorsSpacing.y);
            anchorsSpacingLerp = xLerp > yLerp ? xLerp : yLerp;
        }

        private void GetCameraDistance()
        {
            aspect = currentCameraController ? currentCameraController.camera.aspect : ratioPreview.Ratio();

            switch (distanceCalculation)
            {
                case DistanceCalculation.Simple:
                    defaultDistance = defaultCameraDistance;
                    maxDistance = maxCameraDistance;
                    break;

                case DistanceCalculation.FrustumWidth:
                    defaultDistance = EnhancedMath.GetDistanceFromFrustumHeight(FOV, defaultFrustumWidth / aspect);
                    maxDistance = EnhancedMath.GetDistanceFromFrustumHeight(FOV, maxFrustumWidth / aspect);
                    break;

                case DistanceCalculation.FrustumHeight:
                    defaultDistance = EnhancedMath.GetDistanceFromFrustumHeight(FOV, defaultFrustumHeight);
                    maxDistance = EnhancedMath.GetDistanceFromFrustumHeight(FOV, maxFrustumHeight);
                    break;
            }

            currentCameraDistance = Mathf.Lerp(
                currentCameraDistance,
                soloMode ? defaultDistance : Mathf.Lerp(defaultDistance, maxDistance, anchorsSpacingLerp),
#if UNITY_EDITOR
                Application.isPlaying ? zoomLerpRate * Time.deltaTime : 1);
#else
                zoomLerpRate * Time.deltaTime);
#endif
        }


        private void ApplyTargetPosition()
        {
            position = new Vector3(currentTargetPoint.x, currentTargetPoint.y, 0);
        }


        public override Vector3 GetControllerAnchor(CameraController2D controller)
        {
            return position.ZValue(-currentCameraDistance);
        }


        private Vector2 FramePoint(Vector2 point, Vector2 frameCenter, Vector2 frame)
        {
            return new Bounds(frameCenter, frame).ClosestPoint(point);
        }

        private Vector2 AnchorsEdgeFrame(Vector2 referenceFrame)
        {
            Vector2 frame = referenceFrame - anchorsEdgeOffset * 2;

            if (frame.x < 0f)
                frame.x = 0f;

            if (frame.y < 0f)
                frame.y = 0f;

            return frame;
        }

#endregion
    }
}