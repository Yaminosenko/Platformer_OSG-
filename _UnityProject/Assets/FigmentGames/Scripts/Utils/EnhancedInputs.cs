using UnityEngine;

namespace FigmentGames
{
    [System.Serializable]
    public enum CompactHoldKeyCode
    {
        None,
        Shift,
        Control,
        Alt
    }

    [System.Serializable]
    public struct EnhancedKeyCode
    {
        [SerializeField] private CompactHoldKeyCode _keyHold;
        public CompactHoldKeyCode keyHold { get { return _keyHold; } }

        [SerializeField] private KeyCode _keyPress;
        public KeyCode keyPress { get { return _keyPress; } }
    }

    public abstract class EnhancedInputs
    {
        public static bool IsKeyCombinationValid(EnhancedKeyCode ekc)
        {
            return EnhancedKeyHoldPressed(ekc) && Input.GetKeyDown(ekc.keyPress);
        }

        private static bool EnhancedKeyHoldPressed(EnhancedKeyCode ekc)
        {
            switch (ekc.keyHold)
            {
                case CompactHoldKeyCode.None:
                    return !(ShiftPressed() || ControlPressed() || AltPressed());

                case CompactHoldKeyCode.Shift:
                    return ShiftPressed();

                case CompactHoldKeyCode.Control:
                    return ControlPressed();

                case CompactHoldKeyCode.Alt:
                    return AltPressed();

                default:
                    return false;
            }
        }

        private static bool ShiftPressed()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        private static bool ControlPressed()
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }

        private static bool AltPressed()
        {
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }
    }
}