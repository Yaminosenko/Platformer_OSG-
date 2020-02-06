using UnityEngine;

namespace FigmentGames
{
    using static EnhancedPhysics;

    public static class PolygonCollider2DExtensions
    {
        /// <summary>
        /// Retrieves the shortest vector tu push the given point out of the collider shape.
        /// </summary>
        public static Vector2 PushOutPoint(this PolygonCollider2D collider, Vector2 point)
        {
            // Do not check corners that are outside of the collider shape
            if (!collider.OverlapPoint(point))
                return Vector2.zero;

            // Cache
            float minMagnitude = float.MaxValue;
            Vector2 pathPointsOffset = (Vector2)collider.transform.position + collider.offset;
            Vector2 outVector = Vector2.zero;

            // Calculate shortest vector
            for (int pathIndex = 0; pathIndex < collider.pathCount; pathIndex++)
            {
                Vector2[] pathPoints = collider.GetPath(pathIndex);

                for (int pointIndex = 0; pointIndex < pathPoints.Length; pointIndex++)
                {
                    int nextPoint = pointIndex == pathPoints.Length - 1 ? 0 : pointIndex + 1;
                    Vector2 closestPoint = EnhancedMath.GetNearestPointOnFiniteLine(
                        pathPoints[pointIndex] + pathPointsOffset,
                        pathPoints[nextPoint] + pathPointsOffset,
                        point);

                    Vector2 snapVector = closestPoint - point;

                    if (snapVector.magnitude < minMagnitude)
                    {
                        outVector = snapVector;
                        minMagnitude = snapVector.magnitude;
                    }
                }
            }

            return outVector;
        }

        /// <summary>
        /// Retrieves the shortest vector within the given arched range tu push the given point out of the collider shape.
        /// </summary>
        public static Vector2 PushOutPoint(this PolygonCollider2D collider, Vector2 point, Vector2 rangeStart, float rangeAngle)
        {
            // Do not check corners that are outside of the collider shape
            if (!collider.OverlapPoint(point))
                return Vector2.zero;

            // Cache
            float minMagnitude = 10000f;
            Vector2 pathPointsOffset = (Vector2)collider.transform.position + collider.offset;
            Vector2 outVector = Vector2.zero;

            // Calculate shortest vector
            for (int pathIndex = 0; pathIndex < collider.pathCount; pathIndex++)
            {
                Vector2[] pathPoints = collider.GetPath(pathIndex);

                for (int pointIndex = 0; pointIndex < pathPoints.Length; pointIndex++)
                {
                    int nextPoint = pointIndex == pathPoints.Length - 1 ? 0 : pointIndex + 1;
                    Vector2 closestPoint = EnhancedMath.GetNearestPointOnFiniteLine(
                        pathPoints[pointIndex] + pathPointsOffset,
                        pathPoints[pointIndex == pathPoints.Length - 1 ? 0 : pointIndex + 1] + pathPointsOffset,
                        point);

                    Vector2 snapVector = closestPoint - point;
                    float angle = Vector2.SignedAngle(rangeStart, snapVector);

                    // Snap vector is outside of the arched range
                    if (angle < 0f || angle > rangeAngle)
                        continue;

                    if (snapVector.magnitude < minMagnitude)
                    {
                        outVector = snapVector;
                        minMagnitude = snapVector.magnitude;
                    }
                }
            }

            return outVector;
        }
    }
}