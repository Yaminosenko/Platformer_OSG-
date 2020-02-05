using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Snap), true)]
public class SnapEditor : Editor
{
    private Vector3 prevPositon;
    private Snap snap;

    void OnEnable()
    {
        snap = target as Snap;
    }

    protected void OnSceneGUI()
    {
        if (Application.isPlaying)
            return;

        Vector3 currentPosition = snap.transform.position;
        if (currentPosition != prevPositon)
        {
            prevPositon = currentPosition;
            snap.transform.position = Snap.SnapToGrid(currentPosition);
        }
    }
}
