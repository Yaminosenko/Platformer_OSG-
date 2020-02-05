using UnityEngine;

namespace FigmentGames
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
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
                    Debug.LogError($"No <color=red>{typeof(T)}</color> found in any Resources folder of the project.");
                }

                return _instance ?? null;
            }
        }

        private static T GetAsset()
        {
            Object[] instances = Resources.LoadAll("", typeof(T));

            if (instances.Length > 1)
                Debug.LogWarning($"Multiple <color=red>{typeof(T)}</color> have been found the project. Instance will refer to the first one.");

            if (instances.Length > 0)
                return (T)instances[0];

            return null;
        }
    }
}