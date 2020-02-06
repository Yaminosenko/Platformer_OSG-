using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    public abstract class EditorSingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance)
                    _instance = GetAsset();

                // Asset has not been found
                if (!_instance && Application.isPlaying)
                {
                    Debug.LogError($"No <color=red>{typeof(T)}</color> found in any Editor folder of the project.");
                }

                return _instance ?? null;
            }
        }

        private static T GetAsset()
        {
            var assets = new List<T>();

            string[] editorPaths = AssetDatabase.FindAssets("t:folder Editor");
            for (int e = 0; e < editorPaths.Length; e++)
            {
                editorPaths[e] = AssetDatabase.GUIDToAssetPath(editorPaths[e]);
            }

            string[] paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}", editorPaths);
            for (int i = 0; i < paths.Length; i++)
            {
                assets.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(T)) as T);
            }

            if (assets.Count != 1)
                return null;

            return assets[0];
        }
    }
}