using System;
using UnityEngine;

namespace FigmentGames
{
    public abstract partial class VirtualCamera : EnhancedMonoBehaviour
    {
        [Space(10)]
        [Header("CONTROLLER PARAMETERS")]
        [Tooltip("This curve represents how smoothly the CameraController2D will blend towards this virtual camera.")]
        [SerializeField] private AnimationCurve _blendCurve = EnhancedMath.easeIn;
        public AnimationCurve blendCurve { get { return _blendCurve; } }

        [Tooltip("The CameraController2D always checks for virtual cameras with the higher priority in its list.")]
        [SerializeField] [MinValue(0)] private int _priority = 0;
        public int priority { get { return _priority; } set { _priority = value; } }

        [Space(10)]
        [Header("CAMERA PARAMETERS")]
        [Tooltip("The FOV of this virtual camera.")]
        [SerializeField] [Range(1, 179)] private int _FOV = 40;
        public int FOV { get { return _FOV; } }

        [Tooltip("A preview screen ratio to display gizmos in the scene view.")]
        [SerializeField] private Vector2 _ratioPreview = new Vector2(16, 9);
        public Vector2 ratioPreview { get { return _ratioPreview; } private set { _ratioPreview = value.Min(1, 1); } }

        [Space]
        [Tooltip(
            "This value determines how is calculated the camera anchor at runtime." +
            "\n\n➜ Simple: The camera anchor is placed at the given distance." +
            "\n\n➜ Frustum Width/Height: The anchor distance is automatically calculated to match the given frustum width/height, taking into account the camera FOV and aspect ratio.")]
        [SerializeField] protected DistanceCalculation distanceCalculation = DistanceCalculation.FrustumWidth;

        public enum DistanceCalculation
        {
            Simple,
            FrustumWidth,
            FrustumHeight
        }


        private void OnEnable()
        {
            Camera2DEvents.OnVirtualCameraSet += CameraSet;
            Camera2DEvents.OnVirtualCameraRemoved += CameraRemoved;
        }

        private void OnDisable()
        {
            Camera2DEvents.OnVirtualCameraSet -= CameraSet;
            Camera2DEvents.OnVirtualCameraRemoved -= CameraRemoved;
        }


        private void CameraSet(CameraController2D controller, VirtualCamera virtualCamera)
        {
            if (virtualCamera != this)
                return;

            VirtualCameraSet(controller);
        }

        private void CameraRemoved(CameraController2D controller, VirtualCamera virtualCamera)
        {
            if (virtualCamera != this)
                return;

            VirtualCameraRemoved();
        }

        protected virtual void VirtualCameraSet(CameraController2D controller) { }
        protected virtual void VirtualCameraRemoved() { }

        protected virtual void OnValidate()
        {
            // Hard set
            ratioPreview = ratioPreview;
        }

        // Override in children classes
        public virtual Vector3 GetControllerAnchor(CameraController2D controller)
        {
            return Vector3.zero;
        }
    }
}