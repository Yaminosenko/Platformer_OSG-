using UnityEngine;
using System.Collections;

namespace FigmentGames
{
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Returns the latest keyframe of the curve.
        /// </summary>
        public static Keyframe GetLastKey(this AnimationCurve curve)
        {
            if (curve.length == 0)
                return new Keyframe();

            return curve.keys[curve.length - 1];
        }

        /// <summary>
        /// Returns the time of the last keyframe.
        /// </summary>
        public static float GetDuration(this AnimationCurve curve)
        {
            return curve.GetLastKey().time;
        }

        /// <summary>
        /// Gets value at time t regardless the curve's wrapping mode.
        /// </summary>
        /// <param name="curve">The curve to sample.</param>
        /// <param name="t">The time at wich the curve will be evaluated.</param>
        public static float EvaluateOutsideBounds(this AnimationCurve curve, float t)
        {
            // Curve has no key
            if (curve.keys.Length == 0)
            {
                return 0f;
            }
            else if (curve.keys.Length == 1) // Curve has one single key
            {
                return curve.keys[0].value;
            }

            // Requested time is actually within the curve's range
            Keyframe firstKey = curve.keys[0];
            Keyframe lastKey = curve.GetLastKey();

            if (t >= firstKey.time && t <= lastKey.time)
            {
                return curve.Evaluate(t);
            }

            // Actual function
            float value = 0f;
            if (t < firstKey.time)
            {
                float timeOffset = t - firstKey.time;
                value = firstKey.value + firstKey.outTangent * timeOffset;
            }
            else
            {
                float timeOffset = t - lastKey.time;
                value = lastKey.value + lastKey.inTangent * timeOffset;
            }

            return value;
        }

        public static Keyframe[] GetScaledKeys(this AnimationCurve curve, Vector2 scale)
        {
            Keyframe[] keys = curve.keys;

            if (scale.x != 0f)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    Keyframe key = keys[i];

                    key.time *= scale.x;
                    key.value *= scale.y;

                    key.inTangent /= scale.x / scale.y;
                    key.outTangent /= scale.x / scale.y;

                    keys[i] = key;
                }
            }

            return keys;
        }
    }
}