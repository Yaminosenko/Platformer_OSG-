using UnityEngine;
using UnityEditor;
using FigmentGames;

namespace FigmentGames
{
    public class TimeManagerWindow : EditorWindow
    {
        private static float _timeScale = 1f;
        private static float timeScale
        {
            get
            {
                return _timeScale;
            }
            set
            {
                if (value < 0f)
                    value = 0f;
                else if (value > 10f)
                    value = 10f;

                _timeScale = value;
            }
        }
        private static int targetFramerate = 30;
        private static int newTargetFramerate = 30;

        [MenuItem("Figment Games/Time Manager/Open Time Manager window %#&t")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TimeManagerWindow), false, "Time Manager");

            GetTimeInfo();
        }

        private static void GetTimeInfo()
        {
            timeScale = Time.timeScale;
            targetFramerate = newTargetFramerate = Application.targetFrameRate;
        }

        [MenuItem("Figment Games/Time Manager/Increase time scale %#&UP")]
        private static void IncreaseTimeScale()
        {
            timeScale += timeScale < 1f ? 0.1f : 0.5f;
            UpdateTimeScale(true);
        }

        [MenuItem("Figment Games/Time Manager/Decrease time scale %#&DOWN")]
        private static void DecreaseTimeScale()
        {
            timeScale -= timeScale <= 1f ? 0.1f : 0.5f;
            UpdateTimeScale(true);
        }

        [MenuItem("Figment Games/Time Manager/Stop time scale %#&0")]
        private static void StopTimeScale()
        {
            timeScale = 0f;
            UpdateTimeScale();
        }

        [MenuItem("Figment Games/Time Manager/Reset time scale %#&1")]
        private static void ResetTimeScale()
        {
            timeScale = 1f;
            UpdateTimeScale();
        }

        private static void UpdateTimeScale(bool roundValue = false)
        {
            if (roundValue)
            {
                if (timeScale < 1f)
                    timeScale = timeScale.RoundValue(0.1f);
                else
                    timeScale = timeScale.RoundValue(0.5f);
            }

            Time.timeScale = timeScale;

            Debug.LogWarning($"Time scale set to {timeScale.ToString("F2")}");
        }

        private static void SetNewTargetFramerate(int fps)
        {
            if (!Application.isPlaying)
                Debug.LogWarning("Framerate settings can only be edited in runtime.");
            else
                newTargetFramerate = fps;
        }

        /*public static void Shortcuts()
        {
            // Shortcuts
            if (shortcuts && Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.KeypadPlus)
                    IncreaseTimeScale();

                if (Event.current.keyCode == KeyCode.KeypadMinus)
                    DecreaseTimeScale();

                if (Event.current.keyCode == KeyCode.Keypad0)
                    StopTimeScale();

                if (Event.current.keyCode == KeyCode.Keypad1)
                    ResetTimeScale();
            }
        }*/

        private void OnGUI()
        {
            // Main cache
            GetTimeInfo();
            bool fpsButton = false;
            bool shortcuts = true;

            // Time scale
            EditorGUI.BeginChangeCheck();
            {
                EnhancedEditor.SmallSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Time scale", GUILayout.Width(140));
                    timeScale = EditorGUILayout.Slider(timeScale, 0f, 10f);

                }
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                UpdateTimeScale();
            }

            // Target framerate
            GUILayout.BeginHorizontal();
            {
                // Cache
                newTargetFramerate = targetFramerate;

                GUILayout.Label("Target framerate", GUILayout.Width(140));

                GUI.enabled = Application.isPlaying;

                if (GUILayout.Button("30 FPS", GUILayout.Width(80), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    SetNewTargetFramerate(30);
                    fpsButton = true;
                }
                if (GUILayout.Button("60 FPS", GUILayout.Width(80), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    SetNewTargetFramerate(60);
                    fpsButton = true;
                }
                if (GUILayout.Button("∞", GUILayout.Width(80), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    SetNewTargetFramerate(-1);
                    fpsButton = true;
                }

                GUILayout.Space(10);
                int customFPS = EditorGUILayout.IntField(targetFramerate);
                if (customFPS < 1)
                    customFPS = -1;
                if (!fpsButton && newTargetFramerate != customFPS)
                    SetNewTargetFramerate(customFPS);

                // Custom value
                if (newTargetFramerate != targetFramerate)
                {
                    if (!Application.isPlaying)
                        return;

                    shortcuts = false; // Disabling shortcuts because user perhaps typed a value manually

                    targetFramerate = newTargetFramerate;
                    Application.targetFrameRate = targetFramerate;

                    QualitySettings.vSyncCount = targetFramerate == -1 ? 1 : 0;

                    Debug.LogWarning($"Application's target framerate set to {targetFramerate} FPS.");
                }
            }
            GUILayout.EndHorizontal();

            // Shortcuts
            /*if (shortcuts)
                Shortcuts();*/
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}