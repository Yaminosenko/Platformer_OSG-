using UnityEngine;

namespace FigmentGames
{
    /// Create inherited class for game-specific custom events
    public class EventsManager : MonoBehaviour
    {
        // Default public delegates
        public delegate void DefaultCallback();
        public delegate void IntCallback(int intValue);
        public delegate void FloatCallback(float floatValue);
        public delegate void BoolCallback(bool boolValue);
        public delegate void RaycastHitCallback(RaycastHit raycastHit);

        /// Add new delegates in inherited class like this:
        ///
        /// public delegate void CustomParameterCallback(CustomParameter customParameter);

        /// Then create custom callbacks like this:
        ///
        /// public static DefaultCallback OnSomethingHappened;
        /// public static CustomParameterCallback OnSomethingElseHappened;
        /// 
        /// Making them static allow using them without any singleton pattern

        /// Don't forget to subscribe and unsubscribe to new custom events in the SUBSCRIPTION region

        #region SUBSCRIPTION

        // Auto subscribe to ALL events to prevent NullReferenceException when calling uninitialized events
        protected virtual void OnEnable()
        {
            // Override in inherited class

            /// Custom event subscription example:
            ///
            /// OnSomethingElseHappened += CustomParameterEvents;
        }

        // Auto unsubscribe to ALL events
        protected virtual void OnDisable()
        {
            // Override in inherited class

            /// Custom event unsubscription example:
            ///
            /// OnSomethingElseHappened -= CustomParameterEvents;
        }

        #endregion

        #region EMPTY FUNCTIONS

        protected void DefaultEvent() { }
        protected void IntEvent(int intValue) { }
        protected void FloatEvent(float floatValue) { }
        protected void BoolEvent(bool boolValue) { }
        protected void RaycastHitEvent(RaycastHit raycastHit) { }

        /// Example empty function for a custom event:
        ///
        /// protected void CustomParameterEvent(CustomParameter customParameter) { }

        #endregion
    }
}