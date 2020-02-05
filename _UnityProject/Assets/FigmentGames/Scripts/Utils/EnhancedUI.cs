using UnityEngine;

namespace FigmentGames
{
    public abstract class EnhancedUI
    {
        /// <summary>
        /// Returns the world position of the specified RectTransform's center.
        /// </summary>
        public static Vector3 RectTransformCenterToWorldPosition(RectTransform rectTransform)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            // Two opposite positions is enough to get the barycenter
            return (worldCorners[0] + worldCorners[2]) / 2f;
        }

        /// <summary>
        /// Returns a string splitted with the specified character.
        /// </summary>
        public static string StringSplit(string text, string character = ".", int step = 3)
        {
            if (step < 1)
                step = 1;

            string stringSplit = text;

            for (int i = stringSplit.Length - step; i > 0; i -= step)
            {
                stringSplit = stringSplit.Insert(i, character);
            }

            return stringSplit;
        }

        /// <summary>
        /// Converts an int value to a string splitted with the specified character.
        /// </summary>
        public static string IntSplit(int value, string character = ".", int step = 3)
        {
            return StringSplit(value.ToString(), character, step);
        }

        /// <summary>
        /// Returns true if the specified screen position is within a rect transform regardless of its transform hierarchy.
        /// </summary>
        public static bool IsPositionWithinRectTransform(RectTransform rectTransform, Canvas rootCanvas, RectTransform rootCanvasRT, Vector2 position)
        {
            Vector2 posFromMasterParent = rootCanvasRT.worldToLocalMatrix * (rectTransform.position - rootCanvasRT.position);
            posFromMasterParent -= rootCanvasRT.rect.min;
            Vector2 globalRectScale = rectTransform.lossyScale / (Vector2)rootCanvasRT.localScale;
            Vector2 bottomLeftPoint = posFromMasterParent + (rectTransform.rect.min * globalRectScale);
            Vector2 globalRectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * globalRectScale;
            Rect zoneRect = new Rect(bottomLeftPoint.x, bottomLeftPoint.y, globalRectSize.x, globalRectSize.y);

            return zoneRect.Contains(position / rootCanvas.scaleFactor);
        }
    }
}