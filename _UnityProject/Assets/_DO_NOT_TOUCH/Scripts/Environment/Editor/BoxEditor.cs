using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Box))]
public class BoxEditor : Editor
{
    private Box box;
    protected class ShapeEditor
    {
        public static Box boxParent;
        public Rect shape;
        public Vector3 worldPosition;

        // EDGES
        private Vector3 topHandlePosition;
        private Vector3 bottomHandlePosition;
        private Vector3 leftHandlePosition;
        private Vector3 rightHandlePosition;

        // CORNERS HANDLES
        public Vector3 topLeftHandlePosition;
        public Vector3 topRightHandlePosition;
        public Vector3 bottomLeftHandlePosition;
        public Vector3 bottomRightHandlePosition;

        // CONSTRUCTOR
        public ShapeEditor(Rect shape, Vector3 worldPosition)
        {
            this.shape = shape;
            this.worldPosition = worldPosition;

            ResetHandlesPositions();
        }

        public void DrawHandles()
        {
            Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;

            float handleSize = HandleUtility.GetHandleSize(worldPosition) * 0.3f;
            float horizontalHandleSize = (sceneViewCamera.WorldToScreenPoint(topLeftHandlePosition) - sceneViewCamera.WorldToScreenPoint(topRightHandlePosition)).magnitude * handleSize * 0.02f;
            float verticalHandleSize = (sceneViewCamera.WorldToScreenPoint(topLeftHandlePosition) - sceneViewCamera.WorldToScreenPoint(bottomLeftHandlePosition)).magnitude * handleSize * 0.02f;

            Vector3 previous_topHandlePosition = topHandlePosition;
            Vector3 previous_bottomHandlePosition = bottomHandlePosition;
            Vector3 previous_leftHandlePosition = leftHandlePosition;
            Vector3 previous_rightHandlePosition = rightHandlePosition;

            Vector3 previous_topLeftHandlePosition = topLeftHandlePosition;
            Vector3 previous_topRightHandlePosition = topRightHandlePosition;
            Vector3 previous_bottomLeftHandlePosition = bottomLeftHandlePosition;
            Vector3 previous_bottomRightHandlePosition = bottomRightHandlePosition;

            EditorGUI.BeginChangeCheck(); // Handles start here
            Handles.color = Color.yellow;

            // lines
            if (SceneView.currentDrawingSceneView.in2DMode)
            { 
                topHandlePosition = Handles.Slider(topHandlePosition + worldPosition, Vector3.up, horizontalHandleSize, Handles.RectangleHandleCap, 10f) - worldPosition;
                bottomHandlePosition = Handles.Slider(bottomHandlePosition + worldPosition, Vector3.up, horizontalHandleSize, Handles.RectangleHandleCap, 10f) - worldPosition;
                leftHandlePosition = Handles.Slider(leftHandlePosition + worldPosition, Vector3.right, verticalHandleSize, Handles.RectangleHandleCap, 10f) - worldPosition;
                rightHandlePosition = Handles.Slider(rightHandlePosition + worldPosition, Vector3.right, verticalHandleSize, Handles.RectangleHandleCap, 10f) - worldPosition;
            }

            
            topLeftHandlePosition = Handles.Slider2D(topLeftHandlePosition + worldPosition, Vector3.back, Vector3.right, Vector3.up, handleSize, Handles.SphereHandleCap, 10f) - worldPosition;
            topRightHandlePosition = Handles.Slider2D(topRightHandlePosition + worldPosition, Vector3.back, Vector3.right, Vector3.up, handleSize, Handles.SphereHandleCap, 10f) - worldPosition;
            bottomLeftHandlePosition = Handles.Slider2D(bottomLeftHandlePosition + worldPosition, Vector3.back, Vector3.right, Vector3.up, handleSize, Handles.SphereHandleCap, 10f) - worldPosition;
            bottomRightHandlePosition = Handles.Slider2D(bottomRightHandlePosition + worldPosition, Vector3.back, Vector3.right, Vector3.up, handleSize, Handles.SphereHandleCap, 10f) - worldPosition;

            if (EditorGUI.EndChangeCheck())
            {
                if (previous_topHandlePosition != topHandlePosition)
                {
                    topLeftHandlePosition.y = topHandlePosition.y;
                    topRightHandlePosition.y = topHandlePosition.y;
                }
                else if (previous_bottomHandlePosition != bottomHandlePosition)
                {
                    bottomLeftHandlePosition.y = bottomHandlePosition.y;
                    bottomRightHandlePosition.y = bottomHandlePosition.y;
                }
                else if (previous_leftHandlePosition != leftHandlePosition)
                {
                    topLeftHandlePosition.x = leftHandlePosition.x;
                    bottomLeftHandlePosition.x = leftHandlePosition.x;
                }
                else if (previous_rightHandlePosition != rightHandlePosition)
                {
                    topRightHandlePosition.x = rightHandlePosition.x;
                    bottomRightHandlePosition.x = rightHandlePosition.x;
                }

                if (previous_topLeftHandlePosition != topLeftHandlePosition)
                {
                    topRightHandlePosition.y = topLeftHandlePosition.y;
                    bottomLeftHandlePosition.x = topLeftHandlePosition.x;
                }
                else if (previous_topRightHandlePosition != topRightHandlePosition)
                {
                    topLeftHandlePosition.y = topRightHandlePosition.y;
                    bottomRightHandlePosition.x = topRightHandlePosition.x;
                }
                else if (previous_bottomLeftHandlePosition != bottomLeftHandlePosition)
                {
                    bottomRightHandlePosition.y = bottomLeftHandlePosition.y;
                    topLeftHandlePosition.x = bottomLeftHandlePosition.x;
                }
                else if (previous_bottomRightHandlePosition != bottomRightHandlePosition)
                {
                    bottomLeftHandlePosition.y = bottomRightHandlePosition.y;
                    topRightHandlePosition.x = bottomRightHandlePosition.x;
                }

                topLeftHandlePosition = Snap.SnapToGrid(topLeftHandlePosition);
                topRightHandlePosition = Snap.SnapToGrid(topRightHandlePosition);
                bottomLeftHandlePosition = Snap.SnapToGrid(bottomLeftHandlePosition);
                bottomRightHandlePosition = Snap.SnapToGrid(bottomRightHandlePosition);


                ApplyHandlesToShape();
            }
        }

        public void ApplyHandlesToShape()
        {
            shape.width = topRightHandlePosition.x - topLeftHandlePosition.x;
            shape.height = topLeftHandlePosition.y - bottomLeftHandlePosition.y;

            shape.x = bottomLeftHandlePosition.x;
            shape.y = bottomLeftHandlePosition.y;

            Undo.RecordObject(boxParent, "Change " + boxParent.name + " shape.");
            boxParent.SetShape(shape);

            ResetHandlesPositions();

            List<Vector2> vertices = new List<Vector2>(4)
            {
                bottomRightHandlePosition,
                bottomLeftHandlePosition,
                topLeftHandlePosition,
                topRightHandlePosition
            };

            boxParent.GenerateMesh(vertices);

        }

        protected void ResetHandlesPositions()
        {
            Vector3 centerPosition = shape.center;

            topHandlePosition = centerPosition + new Vector3(0, shape.height / 2f);
            bottomHandlePosition = centerPosition + new Vector3(0, shape.height / -2f);
            leftHandlePosition = centerPosition + new Vector3(shape.width / -2f, 0);
            rightHandlePosition = centerPosition + new Vector3(shape.width / 2f, 0);

            topLeftHandlePosition = centerPosition + new Vector3(shape.width / -2f, shape.height / 2f);
            topRightHandlePosition = centerPosition + new Vector3(shape.width / 2f, shape.height / 2f);
            bottomLeftHandlePosition = centerPosition + new Vector3(shape.width / -2f, shape.height / -2f);
            bottomRightHandlePosition = centerPosition + new Vector3(shape.width / 2f, shape.height / -2f);
        }
    }
    protected ShapeEditor boxShape;

    private bool pivotModeChanged;

    private void OnEnable()
    {
        box = target as Box;
        Initialize();
    }

    private void OnDisable()
    {
        if (pivotModeChanged)
            Tools.pivotMode = PivotMode.Pivot;
    }

    protected void Initialize()
    {
        ShapeEditor.boxParent = box;
        Vector3 boxPosition = box.transform.position;

        Rect shape = box.GetShape();
        boxShape = new ShapeEditor(shape, boxPosition);

        if (Tools.pivotMode == PivotMode.Pivot)
        {
            pivotModeChanged = true;
            Tools.pivotMode = PivotMode.Center;
            SceneView.RepaintAll();
        }

        boxShape.ApplyHandlesToShape();
    }

#if UNITY_EDITOR
    protected void OnSceneGUI()
    {
        if (!box)
            return;

        if (Application.isPlaying)
            return;

        if (boxShape.shape != box.GetShape())
        {
            Initialize();
            return;
        }

        if (Tools.current != Tool.Move)
            Tools.current = Tool.Move;


        Vector3 boxCurrentPosition = box.transform.position;
        if (boxCurrentPosition != prevPosition)
        {
            prevPosition = boxCurrentPosition;
            box.transform.position = Snap.SnapToGrid(boxCurrentPosition, .5f, .5f, false, .5f);
        }

        boxShape.worldPosition = boxCurrentPosition;
        boxShape.DrawHandles();
    }
#endif

    private Vector3 prevPosition;
}
