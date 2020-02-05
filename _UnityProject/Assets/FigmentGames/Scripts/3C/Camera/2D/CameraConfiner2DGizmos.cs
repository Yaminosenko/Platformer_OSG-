#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    public partial class CameraConfiner2D : CameraConstraint2D
    {
        [MenuItem("GameObject/Figment Games/Camera/Camera Confiner 2D", false, 0)]
        static void CreateCameraController2D(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraConfiner2D");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add the component
            go.AddComponent<CameraConfiner2D>();

            // Change the default gizmos color
            go.GetComponent<EditablePolygonCollider>().shapeColor = Color.green.Alpha(0.5f);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif