using UnityEngine;

namespace FigmentGames
{
    [RequireComponent(typeof(BezierCurve))]
    public partial class CameraPath2D : CameraConstraint2D
    {
        [SerializeField, HideInInspector] private BezierCurve _curve;
        public BezierCurve curve
        {
            get
            {
                if (!_curve)
                    _curve = GetComponent<BezierCurve>();

                return _curve;

            }

            private set
            {
                _curve = curve;
            }
        }

        [Space(10)]
        [Header("BEHAVIOUR")]
        [Space]
        [Tooltip(
            "This value determines how the curve can be manipulated." +
            "\n\n➜ Flat: The curve can only be manipulated in 2D space (XY)." +
            "\n\n➜ Unconstrained: The curve can be manipulated without any constraint. The Z depth works as a distance offset on all cameras that have this path applied.")]
        [SerializeField] private CurveMode _curveMode;
        public CurveMode curveMode { get { return _curveMode; } }

        [Space]
        [Tooltip("This value determines how strongly the camera is snapped to the curve.")]
        [SerializeField] [MinValue(1)] private float snapStrength = 4f;

        public enum CurveMode
        {
            Flat,
            Unconstrained
        }

        // Cache
        private Vector3 targetPoint;
        private Vector3 previousTargetPoint;
        private Vector3 previousControllerVirtualTargetPoint;
        private float previousTime;

        protected override void ControllerAssigned(CameraController2D controller)
        {
            previousTime = Time.time;
            targetPoint = curve.GetNearestPointOnSegmentPath(controller.virtualTargetPoint.ZValue(0)).ZValue(controller.virtualTargetPoint.z);
        }

        public override Vector3 GetConstraintPosition(CameraController2D controller)
        {
            // Target position and cache
            Vector3 cacheVirtualTargetPoint = curveMode == CurveMode.Flat ? controller.virtualTargetPoint.ZValue(0) : controller.virtualTargetPoint;
            Vector3 snapPoint = cacheVirtualTargetPoint;
            if (curveMode == CurveMode.Flat)
            {
                snapPoint = curve.GetNearestPointOnSegmentPath(cacheVirtualTargetPoint);
            }
            else
            {
                Vector3[] intersections = curve.GetPlaneIntersectionsWithSegmentPath(new Plane(Vector3.right, cacheVirtualTargetPoint), 1);
                snapPoint = intersections.Length == 0 ? cacheVirtualTargetPoint.ZValue(0) : intersections[0];
            }
            
            Vector3 deltaPoint = snapPoint - previousTargetPoint;
            Vector3 controllerDeltaPosition = cacheVirtualTargetPoint - previousControllerVirtualTargetPoint;
            float deltaTime = Time.time - previousTime;
            float smoothLerp = deltaPoint.magnitude < 0.01f ? 1f : Mathf.Clamp(controllerDeltaPosition.magnitude / deltaPoint.magnitude, snapStrength * deltaTime, 1f);

            // Edit target point
            targetPoint += controllerDeltaPosition;
            targetPoint = Vector3.Lerp(
                targetPoint,
                snapPoint,
#if UNITY_EDITOR
                Application.isPlaying ? smoothLerp : 1);
#else
                smoothLerp);
#endif

            // Late cache for delta calculations
            previousTime = Time.time;
            previousTargetPoint = targetPoint;
            previousControllerVirtualTargetPoint = cacheVirtualTargetPoint;

            return targetPoint.ZOffset(controller.virtualTargetPoint.z);
        }
    }
}