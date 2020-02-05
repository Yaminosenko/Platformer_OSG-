using UnityEngine;

namespace FigmentGames
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (Quitting)
                    return null;

                return GetInstance() ? _instance : null;
            }
        }

        [SerializeField] private bool _persistent = true;

        protected virtual void Awake()
        {
            // Delete gameobject if static instance is already set
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // The gameobject needs to be unparented to go to DontDestroyOnLoad scene
            if (_persistent)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            GetInstance();

            OnAwake();
        }

        protected virtual void OnAwake() { }

        private static bool GetInstance()
        {
            if (_instance != null)
                return true;

            // Get all instances
            var instances = FindObjectsOfType<T>();

            // No instance has been found
            if (instances.Length == 0)
                return false;

            // Destroy all possible other occurencies
            for (int i = 1; i < instances.Length; i++)
                Destroy(instances[i].gameObject);

            _instance = instances[0];

            return true;
        }
    }

    public abstract class Singleton : MonoBehaviour
    {
        public static bool Quitting { get; private set; }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
    }
}