#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    public partial class CameraPath2D : CameraConstraint2D
    {
        private void OnValidate()
        {
            // Hard serialization
            curve = curve;
        }

        [MenuItem("GameObject/Figment Games/Camera/Camera Path 2D", false, 0)]
        static void CreateCameraController2D(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraPath2D");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add the component
            go.AddComponent<CameraPath2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif