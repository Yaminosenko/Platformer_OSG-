using UnityEngine;

namespace FigmentGames
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets the layer of the GameObject.
        /// </summary>
        public static void SetLayer(this GameObject gameObject, int layer, bool setChildrenLayer)
        {
            if (!setChildrenLayer)
            {
                gameObject.layer = layer;
            }
            else
            {
                Transform[] childrenTransforms = gameObject.GetComponentsInChildren<Transform>();

                int childrenCount = childrenTransforms.Length;
                for (int go = 0; go < childrenCount; go++)
                    childrenTransforms[go].gameObject.layer = layer;
            }
        }
    }
}