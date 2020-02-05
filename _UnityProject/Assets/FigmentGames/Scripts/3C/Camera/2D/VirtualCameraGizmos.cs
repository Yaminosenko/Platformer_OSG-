#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    public abstract partial class VirtualCamera : EnhancedMonoBehaviour
    {
        protected virtual void OnDrawGizmos()
        {
            EditorUpdate();
        }

        protected virtual void EditorUpdate()
        {
            
        }
    }
}
#endif