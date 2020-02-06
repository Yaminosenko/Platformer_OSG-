#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace FigmentGames
{
    public partial class EditablePolygonCollider : EnhancedMonoBehaviour
    {
        [Space(10)]
        [Header("EDITOR")]
        [Tooltip("When ticked, the collider shape stays still while the transform is moved.")]
        public bool offsetCompensate = false;
        [Tooltip("The global size of the scene view handles.")]
        [Range(0.5f, 2f)] public float handlesSize = 1f;
        [Tooltip("This value determines the snap spacing of the handles when they are moved.")]
        public Vector2 snap = Vector2.one;
        [Tooltip("When ticked, the snaps is offseted with the current transform translation.")]
        public bool snapRelative;

        [Space(10)]
        [Header("GIZMOS")]
        public bool drawGizmosUnselected = true;
        [Space]
        [OnOff("drawShape", "Shape")] public Color shapeColor = Color.red.Alpha(0.5f);
        [HideInInspector] public bool drawShape = true;
        [SerializeField] private bool displayName = true;

        // Cache
        [SerializeField, HideInInspector] private bool initialized;
        private bool nonManifoldDraw;
        [HideInInspector] public Vector2 previousPosition;

        public struct Face
        {
            public Vector2 vertex1;
            public Vector2 vertex2;
            public Vector2 vertex3;

            public Face(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3)
            {
                this.vertex1 = vertex1;
                this.vertex2 = vertex2;
                this.vertex3 = vertex3;
            }
        }

        public enum Rotation
        {
            CounterClockwise = 1,
            Clockwise = -1
        }


        private void OnDrawGizmos()
        {
            if (!initialized)
                ResetPolygonColliderShape();

            EditorUpdate();

            if (drawGizmosUnselected)
                DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmosUnselected)
                DrawGizmos();
        }


        private void EditorUpdate()
        {
            if (EditorApplication.isPlaying)
                return;

            polygonCollider.isTrigger = true;

            SetTriggerOffset(offset);
        }

        public void SetTriggerOffset(Vector2 offset)
        {
            this.offset = polygonCollider.offset = offset;
        }

        public void ResetTriggerOffset()
        {
            SetTriggerOffset(Vector2.zero);
        }

        public void ResetPolygonColliderShape()
        {
            polygonCollider.points = new Vector2[4]
            {
                new Vector2(-2, -2),
                new Vector2(2, -2),
                new Vector2(2, 2),
                new Vector2(-2, 2)
            };

            initialized = true;
        }

        private void DrawGizmos()
        {
            if (!drawShape)
                return;

            if (displayName)
                EnhancedGizmos.Label(gameObject.name, position, shapeColor, 40, 5);

            if (polygonCollider.pathCount == 0)
                return;

            // Dotted line
            Handles.color = shapeColor.AlphaRelative(0.5f);
            float size = Mathf.Clamp(0.5f, 0.5f, HandleUtility.GetHandleSize(position) / 8f);
            Handles.SphereHandleCap(-1, position, Quaternion.identity, size, EventType.Repaint);
            if (offset.magnitude > 0f)
            {
                Vector2 offsetPosition = (Vector2)position + offset;
                Handles.SphereHandleCap(-1, offsetPosition, Quaternion.identity, size, EventType.Repaint);
                Handles.color = Handles.color.AlphaRelative(0.5f);
                Handles.DrawDottedLine(position, offsetPosition, 2);
            }

            // Cache
            Color color = nonManifoldDraw ? Color.red.Alpha(Mathf.Cos(Time.realtimeSinceStartup * 8) * 0.5f + 0.5f) : shapeColor;
            nonManifoldDraw = false;

            // Draw all paths
            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                // Get path points
                List<Vector2> pathPoints = polygonCollider.GetPath(i).ToList();

                // Empty path
                if (pathPoints.Count == 0)
                    continue;

                // Draw shape faces
                Handles.color = color.AlphaRelative(0.25f);
                Face[] faces = GetShapeFaces(pathPoints.ToArray());

                for (int f = 0; f < faces.Length; f++)
                {
                    Handles.DrawAAConvexPolygon(
                        cachedTransform.TransformPoint(polygonCollider.offset + faces[f].vertex1),
                        cachedTransform.TransformPoint(polygonCollider.offset + faces[f].vertex2),
                        cachedTransform.TransformPoint(polygonCollider.offset + faces[f].vertex3));
                }

                // Convert Vector2 list to Vector3 list
                List<Vector3> pointsList = new List<Vector3>();

                for (int j = 0; j < pathPoints.Count; j++)
                {
                    pointsList.Add(cachedTransform.TransformPoint(polygonCollider.offset + pathPoints[j]));
                }

                // Close shape
                pointsList.Add(pointsList[0]);

                // Draw outline
                Handles.color = color;
                Handles.DrawAAPolyLine(pointsList.ToArray());
            }
        }

        private Face[] GetShapeFaces(Vector2[] pathPoints, Rotation rotation = Rotation.CounterClockwise)
        {
            List<Vector2> pointsList = pathPoints.ToList();
            List<Face> faces = new List<Face>();

            int iterations = 0;
            bool whileBreak = false;
            while (pointsList.Count > 2)
            {
                if (whileBreak)
                {
                    Debug.LogError($"Something bad is happening! The collider shape ({gameObject.name}) has flipped faces!");
                    nonManifoldDraw = true;
                    break;
                }

                if (iterations > 128)
                {
                    Debug.LogError($"Something REALLY bad is happening! One of the collider paths ({gameObject.name}) is nonsense!");
                    nonManifoldDraw = true;
                    break;
                }

                for (int i = 0; i < pointsList.Count - 2; i++)
                {
                    Vector2 p1 = pointsList[i];
                    Vector2 p2 = pointsList[i + 1];
                    Vector2 p3 = pointsList[i + 2];

                    float angle = Vector2.SignedAngle(p2 - p1, p3 - p2);

                    // Acute angle
                    if (angle * (int)rotation > 0f)
                    {
                        bool pointInFace = false;

                        // Check if a vertex is within the face
                        for (int j = 0; j < pointsList.Count; j++)
                        {
                            // Current triangle vertex
                            if (j > i - 1 && j < i + 3)
                                continue;

                            // If a point is within the face, do not consider it
                            if (PointIsInFace(pointsList[j], p1, p2, p3))
                            {
                                pointInFace = true;
                                break;
                            }
                        }

                        if (pointInFace)
                            continue;

                        // Add new face
                        faces.Add(new Face(p1, p2, p3));

                        // Shorten path by removing the middle vertex of the current triangle
                        pointsList.RemoveAt(i + 1);

                        break;
                    }

                    // No valid face to draw in the entire path: geometry is probably non-manifold
                    if (i == pointsList.Count - 3)
                    {
                        whileBreak = true;
                        break;
                    }
                }

                iterations++;
            }

            return faces.ToArray();
        }

        private bool PointIsInFace(Vector2 point, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 pointVector = point - p1;
            float pointAngle = Vector2.SignedAngle(p2 - p1, pointVector);
            float faceAngle = Vector2.SignedAngle(p2 - p1, p3 - p1);

            // Angle outside face angle thresold
            if (pointAngle < 0f || pointAngle > faceAngle)
                return false;

            // Inside triangle = line p1 -> point does not intersect the opposite edge
            return !EnhancedMath.LineSegmentsIntersection(p1, point, p2, p3);
        }
    }
}
#endif