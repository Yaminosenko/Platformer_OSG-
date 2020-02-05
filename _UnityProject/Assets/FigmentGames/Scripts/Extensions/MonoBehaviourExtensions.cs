using UnityEngine;
using System.Collections;

namespace FigmentGames
{
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Stops the specified coroutine and re-assign it.
        /// </summary>
        public static void StopAndStartCoroutine(this MonoBehaviour mono, ref Coroutine coroutine, IEnumerator function)
        {
            if (coroutine != null)
                mono.StopCoroutine(coroutine);

            if (mono.gameObject.activeInHierarchy) // Coroutines can't be started on inactive game objects
                coroutine = mono.StartCoroutine(function);
        }

        /// <summary>
        /// Stops the specified coroutine.
        /// </summary>
        public static void DestroyCoroutine(this MonoBehaviour mono, ref Coroutine coroutine)
        {
            if (coroutine != null)
                mono.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}