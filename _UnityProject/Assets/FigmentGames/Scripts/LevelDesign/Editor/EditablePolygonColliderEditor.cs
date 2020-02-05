using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EditablePolygonCollider), true)]
    public class EditablePolygonColliderEditor : Editor
    {
        private EditablePolygonCollider trigger;

        private bool movingOffsetHandle;

        private void OnEnable()
        {
            trigger = target as EditablePolygonCollider;
        }

        private void OnSceneGUI()
        {
            // Lock transform
            trigger.position = trigger.position.ZValue(0);
            trigger.rotation = Quaternion.identity;
            trigger.localScale = Vector3.one;

            // Trigger offset compensate
            if (trigger.offsetCompensate)
                trigger.SetTriggerOffset(trigger.offset + trigger.previousPosition - (Vector2)trigger.position);

            trigger.previousPosition = trigger.position;

            // Snap clamp
            if (trigger.snap.x < 0f)
                trigger.snap.x = 0f;

            if (trigger.snap.y < 0f)
                trigger.snap.y = 0f;

            // Trigger offset handle
            if (!trigger.drawShape)
                return;

            DrawTriggerOffsetHandle();
            DrawPolygonHandles();

            movingOffsetHandle = false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Undo.RecordObject(trigger.polygonCollider, $"Reset polygon collider shape ({trigger.gameObject.name}).");

            EditorGUI.BeginChangeCheck();
            {
                EnhancedEditor.LargeSpace();

                GUILayout.Label("<b>HELPERS</b>", EnhancedGUI.richText);

                EnhancedEditor.LargeSpace();

                EnhancedEditor.CenteredButton("Reset offset", trigger.ResetTriggerOffset);
                EnhancedEditor.CenteredButton("Reset collider shape", trigger.ResetPolygonColliderShape);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(trigger);
                EditorUtility.SetDirty(trigger.polygonCollider);
            }
        }


        private void DrawTriggerOffsetHandle()
        {
            Undo.RecordObjects(new Object[2] { trigger, trigger.polygonCollider}, $"Editing trigger ({trigger.gameObject.name}) offset with handle.");

            Handles.color = trigger.shapeColor;
            Vector2 worldHandlePos = trigger.cachedTransform.TransformPoint(trigger.offset);

            // Handle
            movingOffsetHandle = NiceCircleHandle(-1, worldHandlePos, out Vector3 outPosition, trigger.handlesSize);
            Vector2 localhandlePos = trigger.cachedTransform.InverseTransformPoint(outPosition);

            // Handle has been used
            if (localhandlePos != trigger.offset)
            {
                trigger.SetTriggerOffset(localhandlePos);

                EditorUtility.SetDirty(trigger);
                EditorUtility.SetDirty(trigger.polygonCollider);
            }

            Undo.FlushUndoRecordObjects();
        }

        private void DrawPolygonHandles()
        {
            if (movingOffsetHandle)
                return;

            Undo.RecordObject(trigger.polygonCollider, $"Editing trigger ({trigger.gameObject.name}) polygon collider with handle.");

            int handleID = 1;

            for (int i = 0; i < trigger.polygonCollider.pathCount; i++)
            {
                Vector2[] path = trigger.polygonCollider.GetPath(i);
                bool pathEdited = false;

                for (int j = 0; j < path.Length; j++)
                {
                    Vector2 worldPointPos = trigger.cachedTransform.TransformPoint(path[j] + trigger.offset);
                    Vector2 snapRelativeOffset = trigger.snapRelative ? new Vector2(trigger.position.x % 1f, trigger.position.y % 1f) : Vector2.zero;
                    bool pointEdit = NiceCircleHandle(handleID, worldPointPos, out Vector3 outPosition, trigger.handlesSize * 0.8f);

                    // Snap world point position
                    if (!trigger.snapRelative && pointEdit)
                    {
                        if (trigger.snap.x > 0f)
                            outPosition.x = Mathf.Round(outPosition.x / trigger.snap.x) * trigger.snap.x;

                        if (trigger.snap.y > 0f)
                            outPosition.y = Mathf.Round(outPosition.y / trigger.snap.y) * trigger.snap.y;
                    }

                    Vector2 localPointPos = (Vector2)trigger.cachedTransform.InverseTransformPoint(outPosition) - trigger.offset;

                    if (trigger.snapRelative && pointEdit)
                    {
                        if (trigger.snap.x > 0f)
                            localPointPos.x = Mathf.Round(localPointPos.x / trigger.snap.x) * trigger.snap.x;

                        if (trigger.snap.y > 0f)
                            localPointPos.y = Mathf.Round(localPointPos.y / trigger.snap.y) * trigger.snap.y;
                    }

                    // Handle has been used
                    if (path[j] != localPointPos)
                    {
                        path[j] = localPointPos;
                        pathEdited = true;

                        break;
                    }

                    handleID++;
                }

                if (pathEdited)
                {
                    trigger.polygonCollider.SetPath(i, path);

                    EditorUtility.SetDirty(trigger);
                    EditorUtility.SetDirty(trigger.polygonCollider);

                    break;
                }
            }

            Undo.FlushUndoRecordObjects();
        }


        private const float defaultScreenSize = 1;

        private bool NiceCircleHandle(int controlID, Vector3 position, out Vector3 outPosition, float screenSize = defaultScreenSize, Vector3 snap = default)
        {
            return NiceCircleHandle(Handles.color, controlID, position, out outPosition, screenSize, snap);
        }

        private bool NiceCircleHandle(Color color, int controlID, Vector3 position, out Vector3 outPosition, float screenSize = defaultScreenSize, Vector3 snap = default)
        {
            // Cache
            Color startColor = Handles.color;
            bool click = GUIUtility.hotControl == controlID;
            float handleSize = HandleUtility.GetHandleSize(position) * screenSize * 0.1f * (click ? 1f + Mathf.Cos(Time.time * 8) * 0.1f : 1f);

            outPosition = Handles.FreeMoveHandle(controlID, position, Camera.current.transform.rotation, click ? handleSize * 1.25f : handleSize, Vector3.zero, Handles.CircleHandleCap);

            // Fake handle
            if (click)
            {
                Handles.color = Color.yellow;
                Vector3 forward = Camera.current.transform.forward;

                Handles.DrawWireArc(position, forward, Quaternion.Euler(0, 0, 22.5f) * Vector3.right, 45, handleSize);
                Handles.DrawWireArc(position, forward, Quaternion.Euler(0, 0, 22.5f + 90) * Vector3.right, 45, handleSize);
                Handles.DrawWireArc(position, forward, Quaternion.Euler(0, 0, 22.5f + 180) * Vector3.right, 45, handleSize);
                Handles.DrawWireArc(position, forward, Quaternion.Euler(0, 0, 22.5f + 270) * Vector3.right, 45, handleSize);
            }

            Handles.color = startColor;

            return click;
        }
    }
}