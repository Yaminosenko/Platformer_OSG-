using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class LDTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerStay;
    public UnityEvent onTriggerExit;

    private void Awake()
    {
        if (GetComponent<Collider>().isTrigger == false)
            Debug.LogError("The GameObject LDTrigger " + '"' + name + '"' + " needs a trigger.");

        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void OnTriggerEnter(Collider col)
    {
        if (EventIsEmpty(onTriggerEnter))
            return;

        if (IsEligible(col))
            onTriggerEnter.Invoke();
    }

    public void OnTriggerStay(Collider col)
    {
        if (EventIsEmpty(onTriggerStay))
            return;

        if (IsEligible(col))
            onTriggerStay.Invoke();
    }

    public void OnTriggerExit(Collider col)
    {
        if (EventIsEmpty(onTriggerExit))
            return;

        if (IsEligible(col))
            onTriggerExit.Invoke();
    }

    private bool EventIsEmpty(UnityEvent e)
    {
        return e.GetPersistentEventCount() == 0;
    }

    private bool IsEligible(Collider col)
    {
        return col.CompareTag("Player");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        DrawEvents(onTriggerEnter, Color.green, "OnTriggerEnter", 0);
        DrawEvents(onTriggerStay, Color.cyan, "OnTriggerStay", 0.2f);
        DrawEvents(onTriggerExit, Color.red, "OnTriggerExit", 0.4f);
    }
    private GUIStyle labelStyle;

    private void DrawEvents(UnityEvent e, Color color, string name, float pos)
    {
        Gizmos.color = color;

        int eventCount = e.GetPersistentEventCount();
        if (eventCount == 0) return;

        // Draw header 
        {
            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.black;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 11;
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(1, 1, color);
            t.Apply();
            labelStyle.normal.background = t;

            Handles.Label(transform.position + new Vector3(0, 1 + pos, 0), name, labelStyle);
        }

        var offset = 0f;
        for (int i = 0; i < eventCount; i++)
        {
            if (e.GetPersistentTarget(i) == null)
                return;

            string targetName = e.GetPersistentTarget(i).name;
            Transform targetTransform = GameObject.Find(targetName).transform;
            Gizmos.DrawLine(transform.position + new Vector3(0, 1 + pos, 0), targetTransform.position);

            // Draw index number & method name  
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.black;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleLeft;
                labelStyle.fontSize = 9;
                Texture2D te = new Texture2D(1, 1);
                te.SetPixel(1, 1, color);
                te.Apply();
                labelStyle.normal.background = te;

                offset += 0.4f;
                string methodName = e.GetPersistentTarget(i).GetType().Name + "." + e.GetPersistentMethodName(i);
                Handles.Label(targetTransform.position + new Vector3(0, 1 + offset + pos * 2.5f, 0), " " + methodName + " (" + targetName + ") ", labelStyle);
            }
        }
    }
#endif
}
