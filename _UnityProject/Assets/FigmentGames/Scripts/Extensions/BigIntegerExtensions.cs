using UnityEngine;
using System.Numerics;

namespace FigmentGames
{
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Clamps the BigInteger value to the given minimum one.
        /// </summary>
        public static BigInteger Min(this BigInteger value, BigInteger min)
        {
            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Clamps the BigInteger value to the given maximum one.
        /// </summary>
        public static BigInteger Max(this BigInteger value, BigInteger max)
        {
            if (value > max)
                value = max;

            return value;
        }

        /// <summary>
        /// Returns the float rational result of maxValue divided by value.
        /// </summary>
        public static float FloatRational(this BigInteger value, BigInteger maxValue)
        {
            if (value == 0)
                return 0f;

            BigInteger intDiv = BigInteger.DivRem(maxValue, value, out BigInteger remainder);
            if (intDiv > int.MaxValue) // Accuracy too low
                return 0f;
            float floatDiv = remainder == 0 ? 0f : 1f / (float)BigInteger.Divide(maxValue, remainder);
            return 1f / ((int)intDiv + floatDiv);
        }

        /// <summary>
        /// Returns the float rational result of maxValue divided by value.
        /// </summary>
        public static BigInteger RationalCeil(this BigInteger value, int multiplyValue, int divideValue)
        {
            BigInteger outValue = BigInteger.DivRem(BigInteger.Multiply(value, multiplyValue), divideValue, out BigInteger remainder);

            if (remainder > 0)
                outValue++;

            return outValue;
        }
    }
}