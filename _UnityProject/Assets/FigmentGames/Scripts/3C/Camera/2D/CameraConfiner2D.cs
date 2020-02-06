using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FigmentGames
{
    [RequireComponent(typeof(EditablePolygonCollider))]
    public partial class CameraConfiner2D : CameraConstraint2D
    {
        [SerializeField, HideInInspector] private EditablePolygonCollider _shape;
        public EditablePolygonCollider shape
        {
            get
            {
                if (!_shape)
                    _shape = GetComponent<EditablePolygonCollider>();

                return _shape;
            }

            set
            {
                _shape = value;
            }
        }

        [Space(10)]
        [Header("BEHAVIOUR")]
        [Tooltip(
            "Different algorithms to calculate the constraint position that needs to be applied to the camera controller." +
            "\n\n➜ Push Out: Tries to get the longest vector to push each camera corner outside of the confiner collider. Quite reliable." +
            "\n\n➜ Cast: Casts each camera corner towards the confiner surface to get the camera distance and direction from the collider. Not really reliable.")]
        [SerializeField] private SnapMethod snapMethod;
        [Space]
        [Tooltip("The higher this value is, the smoother the snap will occur when the camera controller adjusts its position through complex geometry.")]
        [SerializeField] [MinValue(1)] private float snapStrength = 4f;

        public enum SnapMethod
        {
            PushOut,
            Cast
        }

        // Cache
        private Vector2 currentTargetPoint;
        private Vector2 targetPoint;
        private float previousTime;
        private Vector3[] cameraCorners = new Vector3[4];
        private Vector2[] cameraVectors = new Vector2[4];
        private Vector2 previousTargetPoint;
        private Vector2 previousControllerVirtualTargetPoint;

        protected override void ControllerAssigned(CameraController2D controller)
        {
            previousTime = Time.time;
            targetPoint = previousControllerVirtualTargetPoint = controller.virtualTargetPoint;
            previousTargetPoint = snapMethod == SnapMethod.PushOut ? PushOutMethod(controller) : CastMethod(controller);
        }

        public override Vector3 GetConstraintPosition(CameraController2D controller)
        {
            // Cache camera corners
            cameraCorners = EnhancedMath.GetFrustumCorners(controller.virtualTargetPoint.ZValue(0), Quaternion.identity, controller.camera, Mathf.Abs(controller.virtualTargetPoint.z));

            // Target position and cache
            Vector3 outPoint = snapMethod == SnapMethod.PushOut ? PushOutMethod(controller) : CastMethod(controller);
            Vector2 deltaPoint = (Vector2)outPoint - previousTargetPoint;
            Vector2 controllerDeltaPosition =  (Vector2)controller.virtualTargetPoint - previousControllerVirtualTargetPoint;
            float deltaTime = Time.time - previousTime;
            float smoothLerp = deltaPoint.magnitude < 0.01f ? 1f : Mathf.Clamp(controllerDeltaPosition.magnitude / deltaPoint.magnitude, snapStrength * deltaTime, 1f);

            // Edit target point
            targetPoint += controllerDeltaPosition;
            targetPoint = Vector3.Lerp(targetPoint, outPoint,
#if UNITY_EDITOR
                Application.isPlaying ? smoothLerp : 1);
#else
                smoothLerp);
#endif

            // Late cache for delta calculations
            previousTime = Time.time;
            previousTargetPoint = targetPoint;
            previousControllerVirtualTargetPoint = controller.virtualTargetPoint;

            return new Vector3(targetPoint.x, targetPoint.y, controller.virtualTargetPoint.z);
        }

        private Vector3 PushOutMethod(CameraController2D controller)
        {
            // Get the shortest vector to snap every camera corner to the collider shape
            for (int i = 0; i < 4; i++)
                cameraVectors[i] = shape.polygonCollider.PushOutPoint(cameraCorners[i]);

            //cameraVectors[0] = shape.polygonCollider.PushOutPoint(cameraCorners[0], Vector2.right, 90);
            //cameraVectors[1] = shape.polygonCollider.PushOutPoint(cameraCorners[1], Vector2.down, 90);
            //cameraVectors[2] = shape.polygonCollider.PushOutPoint(cameraCorners[2], Vector2.left, 90);
            //cameraVectors[3] = shape.polygonCollider.PushOutPoint(cameraCorners[3], Vector2.up, 90);

            // Get the longest vector of the 4 camera corners
            Vector3 longestVector = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                if (cameraVectors[i].magnitude > longestVector.magnitude)
                    longestVector = cameraVectors[i];
            }

            return controller.virtualTargetPoint + longestVector;
        }

        private Vector3 CastMethod(CameraController2D controller)
        {
            // Cache
            Vector2 pathPointsOffset = (Vector2)position + shape.polygonCollider.offset;
            int cornersInside = 0;
            Vector3 outVector = Vector3.zero;

            for (int cornerIndex = 0; cornerIndex < 4; cornerIndex++)
            {
                // Do not check corners that are outside of the collider shape
                if (!shape.polygonCollider.OverlapPoint(cameraCorners[cornerIndex]))
                    continue;

                cornersInside++;

                // Per-corner cache
                float maxMagnitude = 0f;
                Vector3 cornerVector = Vector3.zero;

                // Get longest cast
                for (int pathIndex = 0; pathIndex < shape.polygonCollider.pathCount; pathIndex++)
                {
                    Vector2[] pathPoints = shape.polygonCollider.GetPath(pathIndex);

                    for (int pointIndex = 0; pointIndex < pathPoints.Length; pointIndex++)
                    {
                        bool intersection = EnhancedMath.LineSegmentsIntersection(
                            pathPoints[pointIndex] + pathPointsOffset,
                            pathPoints[pointIndex == pathPoints.Length - 1 ? 0 : pointIndex + 1] + pathPointsOffset,
                            controller.virtualTargetPoint,
                            cameraCorners[cornerIndex],
                            out Vector2 intersectionPoint);

                        if (!intersection)
                            continue;

                        Vector2 snapVector = intersectionPoint - (Vector2)cameraCorners[cornerIndex];

                        if (snapVector.magnitude > maxMagnitude)
                        {
                            cornerVector = snapVector;
                            maxMagnitude = snapVector.magnitude;
                        }
                    }
                }

                //cameraVectors[cornerIndex] = cornerVector;

                outVector += cornerVector;
            }

            if (cornersInside > 0)
                outVector /= cornersInside;

            return controller.virtualTargetPoint + outVector;
        }

#if UNITY_EDITOR

        /*private void OnDrawGizmos()
        {
            Handles.color = Color.red;

            for (int i = 0; i < 4; i++)
            {
                Handles.SphereHandleCap(-1, (Vector2)cameraCorners[i] + cameraVectors[i], Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.DrawDottedLine(cameraCorners[i], (Vector2)cameraCorners[i] + cameraVectors[i], 2);
            }
        }*/

        private void OnValidate()
        {
            // Hard serialization
            shape = shape;
        }

#endif
    }
}
