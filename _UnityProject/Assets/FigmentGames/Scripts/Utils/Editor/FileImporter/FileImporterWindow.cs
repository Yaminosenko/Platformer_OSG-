using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FigmentGames
{
    public class FileImporterWindow : EditorWindow
    {
        private List<FileImporterParameters> fips = new List<FileImporterParameters>();
        private FileImporterParameters fip;

        [MenuItem("Figment Games/File Importer %#&i")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FileImporterWindow), false, "File Importer");
        }

        private void OnFocus()
        {
            GetFIP();
        }

        private void OnGUI()
        {
            EnhancedEditor.SmallSpace();

            // User edit in project window
            if (!fip && fips.Count > 0)
                fips = new List<FileImporterParameters>();

            for (int i = 0; i < fips.Count; i++)
            {
                if (fips[i] == null)
                    GetFIP();
            }

            // FIP not found
            if (fips.Count == 0)
            {
                GUILayout.Label("No FileImporterParameters has been found in any Editor folder.", EnhancedGUI.centeredWrapTextStyle);

                EnhancedEditor.SmallSpace();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Create new FileImporterParameters", GUILayout.Width(256), GUILayout.Height(32)))
                    {
                        string path = EditorUtility.OpenFolderPanel("Select a folder", Application.dataPath, "");

                        // Cancel
                        if (string.IsNullOrEmpty(path))
                            return;

                        // Selected folder is not an Editor one
                        if (!path.Contains("/Editor"))
                        {
                            EditorUtility.DisplayDialog(
                                "Incorrect path", "FileImporterParameters must be created in a folder named \"Editor\".\n\nExample: Assets/MyGame/Scriptables/Editor", "Oops...");
                            return;
                        }

                        AssetDatabase.CreateAsset(new FileImporterParameters(), $"{path.Replace(Application.dataPath, "Assets")}/FileImporterParameters.asset");

                        GetFIP();

                        Selection.activeObject = fip;
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();


                return;
            }
            else if (fips.Count > 1)
            {
                // GUI
                GUI.enabled = false;
                GUILayout.BeginVertical("box");
                {
                    GUI.enabled = true;
                    GUILayout.Label("<color=red>Multiple</color> FileImporterParameters have been found in the project.\nPlease keep only one single FileImporterParameters ScriptableObject!", EnhancedGUI.centeredWrapTextStyle);
                }
                GUILayout.EndVertical();

                EnhancedEditor.SmallSpace();

                GUILayout.Label("Scriptables list:");

                GUI.enabled = false;
                for (int i = 0; i < fips.Count; i++)
                    EditorGUILayout.ObjectField($"#{i.ToString("000")}", fips[i], typeof(FileImporterParameters), false);

                return;
            }

            // Actual GUI
            string assetPath = AssetDatabase.GetAssetPath(fip);
            assetPath = assetPath.Replace($"/{fip.name}.asset", "");
            GUILayout.Label($"FileImporterParameters located in <color=orange>{assetPath}</color>.", EnhancedGUI.centeredWrapTextStyle);

            EnhancedEditor.SmallSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                GUI.enabled = Selection.activeObject != fip;
                if (GUILayout.Button("Select FileImporterParameters", GUILayout.Width(256), GUILayout.Height(32)))
                {
                    Selection.activeObject = fip;
                }
                GUI.enabled = true;

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void GetFIP()
        {
            fips = new List<FileImporterParameters>();

            string[] editorPaths = AssetDatabase.FindAssets("t:folder Editor");
            for (int e = 0; e < editorPaths.Length; e++)
            {
                editorPaths[e] = AssetDatabase.GUIDToAssetPath(editorPaths[e]);
            }

            string[] paths = AssetDatabase.FindAssets("t:FileImporterParameters", editorPaths);
            for (int i = 0; i < paths.Length; i++)
            {
                fips.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(FileImporterParameters)) as FileImporterParameters);
            }

            if (fips.Count != 1)
                return;

            fip = fips[0];
        }
    }
}