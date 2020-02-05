using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FigmentGames
{
    [CreateAssetMenu(fileName = "FileImporterParameters", menuName = "Figment Games/File Importer/File Importer Parameters")]
    public class FileImporterParameters : EditorSingletonScriptableObject<FileImporterParameters>
    {
        // Texture
        [Space]
        [Tooltip("If a texture doesn't meet any condition from the conditional texture overrides list, these default settings will be applied at import.")]
        public DefaultTextureSettingsOverride defaultTextureOverrides;

        [Space]
        [Tooltip("Override texture import settings based on nomenclature conditions.")]
        public TextureSettingsOverride[] conditionalTextureOverrides;

        // Model
        [Space]
        [Tooltip("If a model doesn't meet any condition from the conditional model overrides list, these default settings will be applied at import.")]
        public DefaultModelSettingsOverride defaultModelOverrides;

        [Space]
        [Tooltip("Override model import settings based on nomenclature conditions.")]
        public ModelSettingsOverride[] conditionalModelOverrides;

        // Cache
        public bool showInfo = true;

        public enum Condition
        {
            NameContains,
            PathContains,
            IsInFolder,
            HasParentFolder,
            Extension,
            Prefix,
            Suffix
        }

        [System.Serializable]
        public class ConditionParameter
        {
            public Condition condition;
            public string text;
        }

        public static int overrideButtonLength = 72;
        //public static float lineHeight = EditorGUIUtility.singleLineHeight;
        //public static float defaultSpacing = EditorGUIUtility.standardVerticalSpacing;
        public static int spacing = 6;

        public static bool IsConditionValid(ConditionParameter conditionParameter, Object asset)
        {
            if (string.IsNullOrEmpty(conditionParameter.text))
                return false;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            int length = conditionParameter.text.Length;

            switch (conditionParameter.condition)
            {
                case Condition.NameContains:
                    return asset.name.Contains(conditionParameter.text);

                case Condition.PathContains:
                    return assetPath.Contains(conditionParameter.text);

                case Condition.IsInFolder:
                    return assetPath.Contains($"/{conditionParameter.text}/{asset.name}");

                case Condition.HasParentFolder:
                    return assetPath.Contains($"/{conditionParameter.text}/");

                case Condition.Extension:
                    return assetPath.Substring(assetPath.Length - length) == conditionParameter.text;

                case Condition.Prefix:
                    return assetPath.Substring(0, length) == conditionParameter.text;

                case Condition.Suffix:
                    return asset.name.Substring(assetPath.Length - length) == conditionParameter.text;
            }

            return false;
        }

        public static void OverrideField(Rect position, SerializedProperty property, SerializedProperty overrideBool)
        {
            OverrideField(position, property, overrideBool, position.y);
        }

        public static void OverrideField(Rect position, SerializedProperty property, SerializedProperty overrideBool, float y)
        {
            // Cache
            bool GUIEnabled = GUI.enabled;
            int fieldsSpacing = 8;
            Rect fieldRect = new Rect(position.x, y, position.width - overrideButtonLength - fieldsSpacing, EditorGUIUtility.singleLineHeight);
            Rect overrideButtonRect = new Rect(position.x + fieldRect.width + fieldsSpacing, y, overrideButtonLength, EditorGUIUtility.singleLineHeight);

            // Property field
            GUI.color = overrideBool.boolValue ? Color.white : Color.white.Alpha(0.75f);
            GUI.enabled = overrideBool.boolValue;
            EditorGUI.PropertyField(fieldRect, property);

            // Override button
            GUI.enabled = GUIEnabled;
            GUI.color = overrideBool.boolValue ? Color.green : Color.white;
            if (GUI.Button(overrideButtonRect, overrideBool.boolValue ? "Override" : "Dismiss"))
            {
                overrideBool.boolValue = !overrideBool.boolValue;
            }

            GUI.color = Color.white;
        }

        public static float NextLine(float y, bool addSpace = false)
        {
            float newYPos = y + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            return addSpace ? newYPos + spacing : newYPos;
        }
    }

    [CustomPropertyDrawer(typeof(FileImporterParameters.ConditionParameter))]
    public class ConditionParameterDrawer : PropertyDrawer
    {
        private int labelLength = 120;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect indentedRect = EditorGUI.IndentedRect(position);

            Rect conditionRect = position.SetWidth(labelLength);
            conditionRect = position;
            conditionRect.xMax = indentedRect.xMin + labelLength;

            EditorGUI.PropertyField(conditionRect, property.FindPropertyRelative("condition"), GUIContent.none);

            SerializedProperty text = property.FindPropertyRelative("text");

            GUI.color = string.IsNullOrEmpty(text.stringValue) ? new Color(1, 0.5f, 0.5f, 1) : Color.white;

            Rect textFieldRect = conditionRect;
            textFieldRect.x += labelLength + 4;
            textFieldRect.width = position.width - labelLength - 4;
            text.stringValue = EditorGUI.TextField(textFieldRect, text.stringValue);

            GUI.color = Color.white;
        }
    }

    [CustomEditor(typeof(FileImporterParameters))]
    public class FileImporterParametersEditor : Editor
    {
        private FileImporterParameters parameters;
        string parametersPath { get { return AssetDatabase.GetAssetPath(parameters).Replace($"/{parameters.name}.asset", ""); } }

        public override void OnInspectorGUI()
        {
            parameters = target as FileImporterParameters;

            EditorGUI.BeginChangeCheck();
            {
                if (parameters.showInfo)
                {
                    GUILayout.BeginVertical("box");
                    {
                        EnhancedEditor.SmallSpace();

                        if (parameters.showInfo)
                        {
                            GUILayout.Label(
                                "➜ This FileImporterParameters sciptable allows to create and edit <color=orange>projet-specific import settings overrides</color> for models and textures." +
                                "\n\n➜ You can create <color=orange>nomenclature-based conditions</color> to create numerous different import settings overrides." +
                                "\n\n➜ If an imported model or texture does not meet any of the given conditional import settings, the <color=orange>default overrides</color> will be applied." +
                                "\n\n➜ Some hidden parameters are <color=orange>implicitely hard-coded</color> for a simpler version of settings overrides.",
                                EnhancedGUI.richLabelWrapStyle);

                            EnhancedEditor.SmallSpace();
                        }

                        InfoButton();

                        EnhancedEditor.SmallSpace();
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    InfoButton();
                }

                EnhancedEditor.StartCategory(this, "Texture Settings", DrawTexturesCategory);
                EnhancedEditor.StartCategory(this, "Model Settings", DrawModelsCategory);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        private void InfoButton()
        {
            //EnhancedEditor.CenteredButton(parameters.showInfo ? "Hide info" : "Show info", () => { parameters.showInfo = !parameters.showInfo; }, 128);

            if (GUILayout.Button(parameters.showInfo ? "Hide info" : "Show info"))
            {
                parameters.showInfo = !parameters.showInfo;
            }
        }

        private void DrawTexturesCategory()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTextureOverrides"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("conditionalTextureOverrides"), true);

            EnhancedEditor.SmallSpace();
            GUI.enabled = !parameters.defaultTextureOverrides.IsDefault();
            EnhancedEditor.CenteredButton("Reset default texture overrides", () => { parameters.defaultTextureOverrides = new DefaultTextureSettingsOverride(); });
            EnhancedEditor.SmallSpace();
            GUI.enabled = true;
            EnhancedEditor.CenteredButton("Create new TextureSettingsOverride", CreateNewTextureSettingsOverride);
            EnhancedEditor.SmallSpace();
        }

        private void DrawModelsCategory()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultModelOverrides"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("conditionalModelOverrides"), true);

            EnhancedEditor.SmallSpace();
            GUI.enabled = !parameters.defaultModelOverrides.IsDefault();
            EnhancedEditor.CenteredButton("Reset default model overrides", parameters.defaultModelOverrides.Reset);
            EnhancedEditor.SmallSpace();
            GUI.enabled = true;
            EnhancedEditor.CenteredButton("Create new ModelSettingsOverride", CreateNewModelSettingsOverride);
            EnhancedEditor.SmallSpace();
        }

        private void CreateAsset<T>(string path = "") where T : Object, new()
        {
            if (string.IsNullOrEmpty(path))
                path = EditorUtility.OpenFolderPanel("Select a folder", Application.dataPath, "");
            string shortPath = path.Replace(Application.dataPath, "Assets");
            string typeName = typeof(T).Name;

            // Cancel
            if (string.IsNullOrEmpty(path))
                return;

            // Selected folder is not an Editor one
            if (!path.Contains("/Editor"))
            {
                EditorUtility.DisplayDialog("Incorrect path", $"{typeName} must be created in an Editor folder.", "Oops...");
                return;
            }

            // Prevent overriding an asset with the same name
            List<T> list = GetAllAssetsOfType<T>();
            string assetName = typeName;
            bool loop = true;
            while (loop)
            {
                bool sameName = false;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].name == assetName)
                    {
                        assetName = $"New {assetName}";
                        sameName = true;
                        break;
                    }
                }

                if (!sameName)
                    loop = false;
            }

            // Create and select asset
            string newAssetPath = $"{shortPath}/{assetName}.asset";

            AssetDatabase.CreateAsset(new T(), newAssetPath);

            Selection.activeObject = AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(T));
        }

        private List<T> GetAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            List<T> list = new List<T>();
            string[] editorPaths = AssetDatabase.FindAssets("t:folder Editor");
            for (int e = 0; e < editorPaths.Length; e++)
            {
                editorPaths[e] = AssetDatabase.GUIDToAssetPath(editorPaths[e]);
            }

            string[] paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}", editorPaths);
            for (int i = 0; i < paths.Length; i++)
            {
                list.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(T)) as T);
            }

            return list;
        }

        private void CreateNewTextureSettingsOverride()
        {
            CreateAsset<TextureSettingsOverride>(parametersPath);
        }

        private void CreateNewModelSettingsOverride()
        {
            CreateAsset<ModelSettingsOverride>(parametersPath);
        }
    }
}