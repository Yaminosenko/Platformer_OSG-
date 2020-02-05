using System;
using System.Collections;
using UnityEngine;

namespace FigmentGames
{
    public class EnhancedMonoBehaviour : MonoBehaviour
    {
        private Transform _cachedTransform;
        public Transform cachedTransform
        {
            get
            {
                if (!_cachedTransform)
                    _cachedTransform = transform;

                return _cachedTransform;
            }
        }

        public Vector3 position { get { return cachedTransform.position; } set { cachedTransform.position = value; } }
        public Vector3 localPosition { get { return cachedTransform.localPosition; } set { cachedTransform.localPosition = value; } }
        public Quaternion rotation { get { return cachedTransform.rotation; } set { cachedTransform.rotation = value; } }
        public Quaternion localRotation { get { return cachedTransform.localRotation; } set { cachedTransform.localRotation = value; } }
        public Vector3 eulerAngles { get { return cachedTransform.eulerAngles; } set { cachedTransform.eulerAngles = value; } }
        public Vector3 localScale { get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }


        /// <summary>
        /// Invokes an action every frame until the given animation curve's last key time has been reached.
        /// The provided action receives the lerp progress as parameter.
        /// </summary>
        protected void LerpCoroutine(ref Coroutine coroutine, AnimationCurve animationCurve, Action<float> action)
        {
            this.StopAndStartCoroutine(ref coroutine, CoLerpCoroutine(animationCurve, action));
        }

        private IEnumerator CoLerpCoroutine(AnimationCurve animationCurve, Action<float> action)
        {
            float t = 0f;
            float delay = animationCurve.GetLastKey().time;

            while (t < delay)
            {
                t += Time.deltaTime;
                float lerp = animationCurve.Evaluate(t);

                action.Invoke(lerp);

                yield return null;
            }
        }

        /// <summary>
        /// Invokes an action every frame until the given delay has been reached.
        /// The provided action receives the lerp progress as parameter.
        /// </summary>
        protected void LerpCoroutine(ref Coroutine coroutine, float delay, Action<float> action)
        {
            this.StopAndStartCoroutine(ref coroutine, CoLerpCoroutine(delay, action));
        }

        private IEnumerator CoLerpCoroutine(float delay, Action<float> action)
        {
            float t = 0f;

            while (t < delay)
            {
                t += Time.deltaTime;
                float lerp = t / delay;

                action.Invoke(lerp);

                yield return null;
            }
        }
    }
}