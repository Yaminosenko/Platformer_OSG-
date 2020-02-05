using UnityEngine;

namespace FigmentGames
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns bolded text.
        /// </summary>
        public static string Bold(this string text)
        {
            return $"<b>{text}</b>";
        }

        /// <summary>
        /// Returns the string with the given color
        /// </summary>
        public static string Color(this string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
        }

        /// <summary>
        /// Capitalizes the first character of the input string.
        /// </summary>
        public static string Capitalize(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            char c = text[0];

            if (char.IsUpper(c))
                return text;

            return $"{text[0].ToString().ToUpper()}{text.Substring(1)}";
        }

        /// <summary>
        /// Capitalizes the first character of the input string and inserts spaces in-between collapsed words.
        /// </summary>
        public static string NiceName(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Remove ll first non-letter characters
            while (!char.IsLetter(text[0]))
                text = text.Substring(1);

            // Replace all underscores by spaces
            text = text.Replace("_", " ");

            // Capitalize first letter
            text = text.Capitalize();

            int length = text.Length;
            for (int i = 0; i < length - 2; i++)
            {
                char nextChar = text[i + 1];

                // This character is a space and next one is lowercase
                if (text[i] == ' ' && char.IsLower(nextChar))
                {
                    string upperChar = $"{nextChar}".ToUpper();
                    Debug.Log(upperChar);
                    text = text.Remove(i + 1, 1);
                    text = text.Insert(i + 1, upperChar);
                }

                // This character or "i + 2" character is lowercase and next one is uppercase
                if ((char.IsLower(text[i]) || char.IsLower(text[i + 2])) && char.IsUpper(nextChar))
                {
                    text = text.Insert(i + 1, " ");
                    i++;
                }
            }

            return text;
        }

        /// <summary>
        /// Replaces all invalid characters with underscores.
        /// </summary>
        public static string CleanName(this string text, params char[] keepCharacters)
        {
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                char c = text[i];

                bool exception = false;
                for (int param = 0; param < keepCharacters.Length; param++)
                {
                    if (c == keepCharacters[param])
                    {
                        exception = true;
                        break;
                    }
                }

                if (exception)
                    continue;

                if (!char.IsLetterOrDigit(c))
                {
                    text = text.Remove(i, 1);
                    text = text.Insert(i, "_");
                }
            }

            return text;
        }

        public static string Separate(this string text, string separator, int spacing = 3)
        {
            int index = text.Length - 3;

            while (index > 0)
            {
                text = text.Insert(index, separator);
                index -= spacing;
            }

            return text;
        }

        public static string DotSeparate(this string text)
        {
            return Separate(text, ".");
        }

        public static string ComaSeparate(this string text)
        {
            return Separate(text, ",");
        }
    }
}