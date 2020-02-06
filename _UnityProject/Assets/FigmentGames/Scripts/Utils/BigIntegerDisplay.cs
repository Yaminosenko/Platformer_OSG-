using UnityEngine;
using System.Numerics;

namespace FigmentGames
{
    [CreateAssetMenu(fileName = "BigIntegerDisplay", menuName = "Figment Games/BigInteger Display")]
    public class BigIntegerDisplay : ScriptableObject
    {
        [SerializeField] [Range(1, 4)] private int displayAccuracy = 4;
        [SerializeField] private string[] thousandsDisplay;

        public string GetSuffix(BigInteger bigInteger)
        {
            int length = bigInteger.ToString().Length;

            if (length < 4)
                return null;

            int index = (length - 1) / 3 - 1;
            if (index >= thousandsDisplay.Length)
                return "??";

            return thousandsDisplay[index];
        }

        public string GetFullCleanDisplay(BigInteger bigInteger, int displayAccuracy)
        {
            string bigIntString = bigInteger.ToString();
            int length = bigIntString.Length;

            if (length < 4)
                return $"{bigInteger}";

            string cleanString = bigIntString.ComaSeparate().Substring(0, displayAccuracy + 1);
            if (displayAccuracy < 4 && cleanString.EndsWith(","))
                cleanString = cleanString.Remove(displayAccuracy);
            return $"{cleanString}{GetSuffix(bigInteger)}";
        }

        public string GetFullCleanDisplay(BigInteger bigInteger)
        {
            return GetFullCleanDisplay(bigInteger, displayAccuracy);
        }
    }
}