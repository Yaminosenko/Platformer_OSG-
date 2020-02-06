#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    public partial class PolygonTrigger2D : EditablePolygonCollider
    {
        [MenuItem("GameObject/Figment Games/Level Design/Polygon Trigger 2D")]
        static void CreateFixedVirtualCamera2D(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("PolygonTrigger2D");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add the component
            go.AddComponent<PolygonTrigger2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
#endif