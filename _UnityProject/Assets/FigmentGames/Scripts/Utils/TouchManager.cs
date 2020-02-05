using UnityEngine;
using System.Collections.Generic;
using FigmentGames;
using System;

namespace FigmentGames
{
    public class TouchManager : Singleton<TouchManager>
    {
        [Space]
        [SerializeField] protected List<TouchDetectionZone> touchDetectionZones = new List<TouchDetectionZone>();
        [Space]
        [SerializeField] private Canvas mainCanvas;
        private RectTransform _mainCanvasRT;
        private RectTransform mainCanvasRT
        {
            get
            {
                if (!_mainCanvasRT)
                    _mainCanvasRT = mainCanvas.GetComponent<RectTransform>();

                return _mainCanvasRT;
            }
        }
        [Space]
        [SerializeField] [Range(0, 10)] private int maxTouchCount = 2;
        [SerializeField] [Range(0, 10)] private int _touchCount = 0;
        public int touchCount { get { return _touchCount; } }
        [Space]
        [Tooltip("Minimum velocity (cm/s) for swipe detection.")]
        [SerializeField] private float swipeDeltaSpeedThresold = 2;
        [Tooltip("Minimum distance (cm) a swipe needs to travel to be considered.")]
        [SerializeField] private float swipeDragMagnitudeThresold = 1;
        [Space]
        [SerializeField] private int deltaSpeedLerpRate = 8;
        [Space]
        [SerializeField] private List<TouchInfo> _touchInfos = new List<TouchInfo>();
        public List<TouchInfo> touchInfos { get { return _touchInfos; } private set { _touchInfos = value; } }

        [Space]
        [SerializeField] private bool mouseEnabled = false;
        [SerializeField] private bool logEvents = false;

        // Apple iPhone 6 as default
        private int _dpi = 401;
        public int dpi { get { return _dpi; } private set { _dpi = value; } }

        [System.Serializable]
        protected class TouchDetectionZone
        {
            [SerializeField] private RectTransform _touchZone;
            public RectTransform touchZone { get { return _touchZone; } }

            [SerializeField] private bool _enabled = false;
            public bool enabled { get { return _enabled; } }

            public TouchDetectionZone (RectTransform touchZone, bool enabled)
            {
                _touchZone = touchZone;
                _enabled = enabled;
            }

            public void EnableTouchZone(bool enabled)
            {
                _enabled = enabled;
            }
        }

        // Accessors
        private RuntimePlatform platform { get { return Application.platform; } }

        // Delegates
        public delegate void TouchInfoCallback(TouchInfo touchInfo);

        // Events
        public static TouchInfoCallback onTouch;
        public static TouchInfoCallback onTouchDrag;
        public static TouchInfoCallback onTouchRelease;

        #region UNITY

        private void Start()
        {
            dpi = (int)Screen.dpi;
            Debug.Log("Screen DPI: " + dpi);

            touchInfos = new List<TouchInfo>();
            for (int i = 0; i < maxTouchCount; i++)
            {
                touchInfos.Add(new TouchInfo());
            }
        }

        private void Update()
        {
            if (mouseEnabled && (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor))
            {
                GetMouseToTouchInfo();
            }
            else
            {
                GetTouchInfos();
            }

            LerpDeltaSpeed();
        }

        #endregion

        #region BEHAVIOUR

        private void GetTouchInfos()
        {
            Touch[] touches = Input.touches;

            _touchCount = EnhancedMath.IntClamp(touches.Length, 0, maxTouchCount);

            for (int i = 0; i < touches.Length; i++)
            {
                Touch touch = touches[i];

                bool touchInAnyZone = IsTouchWithinAnyZone(mainCanvas, mainCanvasRT, touch.position, out RectTransform touchZone);

                // Touch has not been detected within any touch zone
                if (!touchInAnyZone)
                {
                    if (!TouchEnded(touch))
                    {
                        touch.phase = TouchPhase.Canceled;

                        onTouchRelease.Invoke(touchInfos[touch.fingerId]);
                    }

                    continue;
                }

                // Too many touches on the screen!
                if (touch.fingerId > maxTouchCount - 1)
                    continue;

                if (touch.phase == TouchPhase.Began)
                {
                    WriteStartTouchInfo(touch, touch.position, touchZone);
                    continue;
                }

                WriteTouchInfo(touch, touchZone);

                // Now read the complete touch information
                TouchInfo touchInfo = touchInfos[touch.fingerId];

                if (touchInfo.phase == TouchPhase.Began)
                {
                    onTouch.Invoke(touchInfo);
                }
                else if (touchInfo.phase == TouchPhase.Moved)
                {
                    onTouchDrag.Invoke(touchInfo);
                }
                else if (touchInfo.phase == TouchPhase.Ended)
                {
                    /*touchInfo.touchOutType = IsSwipe(touchInfo) ? TouchOutType.Swipe : TouchOutType.Touch;

                    // Swipe!
                    if (IsSwipe(touchInfo))
                    {
                        TouchReleasedSwipe(touchInfo);

                        Debug.Log("Swipe!");
                    }
                    else // Simple touch
                    {
                        TouchReleased(touchInfo);

                        Debug.Log("Touch!");
                    }*/

                    onTouchRelease.Invoke(touchInfo);
                }
            }
        }

        private bool IsSwipe(TouchInfo touchInfo)
        {
            return touchInfo.screenDragMagnitude > swipeDragMagnitudeThresold &&
                        touchInfo.screenDeltaSpeedLerped > swipeDeltaSpeedThresold;
        }

        private void GetMouseToTouchInfo()
        {
            // Cache
            TouchInfo touchInfo = touchInfos[0];
            Vector2 mousePos = Input.mousePosition;
            bool mouseDown = Input.GetMouseButton(0);
            bool mouseTouch = IsTouchWithinAnyZone(mainCanvas, mainCanvasRT, mousePos, out RectTransform touchZone);
            _touchCount = mouseTouch ? 1 : 0;

            // Mouse has not been detected within any touch zone
            if (!mouseTouch)
            {
                if (!TouchEnded(touchInfo))
                {
                    touchInfo.phase = TouchPhase.Canceled;

                    onTouchRelease.Invoke(touchInfo);
                }

                return;
            }

            // Mouse pressed
            if (Input.GetMouseButtonDown(0))
            {
                touchInfo.phase = TouchPhase.Began;
                touchInfo.startPosition = mousePos;

                WriteTouchInfoFromMouse(true, touchZone);

                onTouch.Invoke(touchInfo);

                return;
            }

            // Read mouse info
            /*if (mouseDown)
            {
                if (touchInfo.phase == TouchPhase.Moved)
                    onTouchDrag.Invoke(touchInfo);

            }
            else // Mouse released
            {
                if (touchInfo.phase != TouchPhase.Ended && touchInfo.phase != TouchPhase.Canceled)
                {
                    touchInfo.phase = TouchPhase.Ended;

                    touchInfo.touchOutType = IsSwipe(touchInfo) ? TouchOutType.Swipe : TouchOutType.Touch;

                    // Swipe!
                    if (IsSwipe(touchInfo))
                    {
                        TouchReleasedSwipe(touchInfo);

                        Debug.Log("Swipe!");
                    }
                    else // Simple touch
                    {
                        TouchReleased(touchInfo);

                        Debug.Log("Touch!");
                    }

                    return;
                }

                onTouchRelease.Invoke(touchInfo);
            }*/

            if (!mouseDown)
            {
                touchInfo.phase = TouchPhase.Ended;

                onTouchRelease.Invoke(touchInfo);

                return;
            }

            if (touchInfo.phase == TouchPhase.Moved)
            {
                onTouchDrag.Invoke(touchInfo);
            }

            if (!TouchEnded(touchInfo))
                WriteTouchInfoFromMouse(false, touchZone);
        }

        private void LerpDeltaSpeed()
        {
            for (int i = 0; i < touchInfos.Count; i++)
            {
                TouchInfo touchInfo = touchInfos[i];

                if (!TouchEnded(touchInfo))
                    touchInfo.deltaSpeedLerped = Mathf.Lerp(touchInfo.deltaSpeedLerped, touchInfo.deltaSpeed, deltaSpeedLerpRate * Time.deltaTime);
            }
        }

        private void WriteTouchInfo(Touch touch, RectTransform touchZone)
        {
            TouchInfo touchInfo = touchInfos[touch.fingerId];

            touchInfos[touch.fingerId].touchZone = touchZone;

            touchInfo.phase = touch.phase;
            touchInfo.position = touch.position;

            touchInfo.deltaPosition = touch.deltaPosition;
            touchInfo.screenDeltaPosition = touchInfo.deltaPosition / dpi * EnhancedMath.inch2cm;
            touchInfo.deltaSpeed = (touch.deltaPosition / Time.deltaTime).magnitude;
            touchInfo.screenDeltaSpeedLerped = touchInfo.deltaSpeedLerped / dpi * EnhancedMath.inch2cm;

            touchInfo.dragVector = touch.position - touchInfo.startPosition;
            touchInfo.dragMagnitude = touchInfo.dragVector.magnitude;
            touchInfo.screenDragMagnitude = touchInfo.dragMagnitude / dpi * EnhancedMath.inch2cm;
        }

        private void WriteStartTouchInfo(Touch touch, Vector2 position, RectTransform touchZone)
        {
            touchInfos[touch.fingerId].startTouchZone = touchZone;
            touchInfos[touch.fingerId].startPosition = position;

            WriteTouchInfo(touch, touchZone);
        }

        private void ResetTouchInfo(Touch touch)
        {
            TouchInfo touchInfo = touchInfos[touch.fingerId];
            touchInfo.phase = TouchPhase.Ended;
            touchInfo.startPosition = Vector2.zero;
            touchInfo.position = Vector2.zero;

            touchInfo.deltaPosition = Vector2.zero;
            touchInfo.screenDeltaPosition = Vector2.zero;
            touchInfo.deltaSpeed = 0;
            touchInfo.deltaSpeedLerped = 0;
            touchInfo.screenDeltaSpeedLerped = 0;

            touchInfo.dragVector = Vector2.zero;
            touchInfo.dragMagnitude = 0;
            touchInfo.screenDragMagnitude = 0;
        }

        private void WriteTouchInfoFromMouse(bool touchBegan, RectTransform touchZone)
        {
            TouchInfo touchInfo = touchInfos[0];
            Vector2 mousePos = Input.mousePosition;

            if (touchBegan)
                touchInfo.startTouchZone = touchZone;
            touchInfo.touchZone = touchZone;

            touchInfo.deltaPosition = touchBegan ? Vector2.zero : mousePos - touchInfo.position;
            touchInfo.screenDeltaPosition = touchInfo.deltaPosition / dpi * EnhancedMath.inch2cm;
            touchInfo.position = mousePos;

            touchInfo.phase = touchBegan ? TouchPhase.Began : (touchInfo.deltaPosition.magnitude == 0 ? TouchPhase.Stationary : TouchPhase.Moved);

            touchInfo.deltaSpeed = touchBegan ? 0 : (touchInfo.deltaPosition / Time.deltaTime).magnitude;
            if (touchBegan)
                touchInfo.deltaSpeedLerped = 0;
            touchInfo.screenDeltaSpeedLerped = touchInfo.deltaSpeedLerped / dpi * EnhancedMath.inch2cm;

            if (touchBegan)
                return;

            touchInfo.dragVector = touchInfo.position - touchInfo.startPosition;
            touchInfo.dragMagnitude = touchInfo.dragVector.magnitude;
            touchInfo.screenDragMagnitude = touchInfo.dragMagnitude / dpi * EnhancedMath.inch2cm;
        }

        private bool TouchEnded(Touch touch)
        {
            return touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        }

        private bool TouchEnded(TouchInfo touchInfo)
        {
            return touchInfo.phase == TouchPhase.Ended || touchInfo.phase == TouchPhase.Canceled;
        }

        private bool IsTouchWithinAnyZone(Canvas rootCanvas, RectTransform rootCanvasRectTransform, Vector2 position, out RectTransform touchZone)
        {
            touchZone = null;

            // A component is missing
            if (!mainCanvas)
                return false;

            // If touch zone list is empty, touch is necessarily on the screen
            if (touchDetectionZones.Count == 0)
                return true;

            for (int i = 0; i < touchDetectionZones.Count; i++)
            {
                TouchDetectionZone zone = touchDetectionZones[i];

                // TouchZone is null or empty
                if (!zone.touchZone)
                    continue;

                // TouchDetectionZone is explicitely disabled
                if (!zone.enabled)
                    continue;

                if (EnhancedUI.IsPositionWithinRectTransform(zone.touchZone, mainCanvas, mainCanvasRT, position))
                {
                    touchZone = zone.touchZone;
                    return true;
                }
            }

            return false;
        }

        public void AddTouchZone(RectTransform touchZone, bool enabled)
        {
            touchDetectionZones.Add(new TouchDetectionZone(touchZone, enabled));
        }

        public void RemoveTouchZone(RectTransform touchZone)
        {
            for (int i = 0; i < touchDetectionZones.Count; i++)
            {
                if (touchDetectionZones[i].touchZone == touchZone)
                {
                    touchDetectionZones.RemoveAt(i);
                    return;
                }
            }
        }

        public void EnableTouchZone(RectTransform touchZone, bool enabled)
        {
            for (int i = 0; i < touchDetectionZones.Count; i++)
            {
                if (touchDetectionZones[i].touchZone == touchZone)
                    touchDetectionZones[i].EnableTouchZone(enabled);

                return;
            }
        }

        public void EnableTouchZone(int touchZoneIndex, bool enabled)
        {
            if (touchZoneIndex < 0 || touchZoneIndex > touchDetectionZones.Count - 1)
                return;

            touchDetectionZones[touchZoneIndex].EnableTouchZone(enabled);
        }

        public void EnableAllTouchZones()
        {
            for (int i = 0; i < touchDetectionZones.Count; i++)
                touchDetectionZones[i].EnableTouchZone(true);
        }

        public void DisableAllTouchZones()
        {
            for (int i = 0; i < touchDetectionZones.Count; i++)
                touchDetectionZones[i].EnableTouchZone(false);
        }

        public void SetTouchZone(RectTransform touchZone)
        {
            for (int i = 0; i < touchDetectionZones.Count; i++)
                touchDetectionZones[i].EnableTouchZone(touchDetectionZones[i].touchZone == touchZone);
        }

        public void SetTouchZone(int touchZoneIndex)
        {
            for (int i = 0; i < touchDetectionZones.Count; i++)
                touchDetectionZones[i].EnableTouchZone(i == touchZoneIndex);
        }

        /*protected virtual void TouchReleased(TouchInfo touchInfo)
        {
            // Per project override
        }

        protected virtual void TouchReleasedSwipe(TouchInfo touchInfo)
        {
            // Per project override
        }*/

        #endregion

        #region EVENTS

        protected virtual void OnEnable()
        {
            onTouch += OnTouch;
            onTouchDrag += OnTouchDrag;
            onTouchRelease += OnTouchRelease;
        }

        protected virtual void OnDisable()
        {
            onTouch -= OnTouch;
            onTouchDrag -= OnTouchDrag;
            onTouchRelease -= OnTouchRelease;
        }

        private void OnTouch(TouchInfo touchInfo) { if (logEvents) Debug.Log("Touch!"); }
        private void OnTouchDrag(TouchInfo touchInfo) { if (logEvents) Debug.Log("Touch drag"); }
        private void OnTouchRelease(TouchInfo touchInfo) { if (logEvents) Debug.Log("Touch release"); }

        #endregion
    }

    [System.Serializable]
    public class TouchInfo
    {
        [Space]
        public RectTransform startTouchZone;
        public RectTransform touchZone;
        [Space]
        public TouchPhase phase = TouchPhase.Ended;
        public Vector2 startPosition = Vector2.zero;
        public Vector2 position = Vector2.zero;
        [Space]
        public Vector2 deltaPosition = Vector2.zero;
        public Vector2 screenDeltaPosition = Vector2.zero;
        public float deltaSpeed = 0;
        public float deltaSpeedLerped = 0;
        public float screenDeltaSpeedLerped = 0;
        [Space]
        public Vector2 dragVector = Vector2.zero;
        public float dragMagnitude = 0;
        public float screenDragMagnitude = 0;
        [Space]
        public TouchOutType touchOutType;
    }

    [System.Serializable]
    public enum TouchOutType
    {
        Touch,
        Swipe
    }
}