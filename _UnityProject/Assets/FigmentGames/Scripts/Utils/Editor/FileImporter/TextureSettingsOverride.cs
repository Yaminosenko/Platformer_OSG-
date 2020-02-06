using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    using static FileImporterParameters;

    [CreateAssetMenu(fileName = "TextureSettingsOverride", menuName = "Figment Games/File Importer/Texture Settings Override")]
    public class TextureSettingsOverride : ScriptableObject
    {
        [Space]
        [Tooltip("When using multiple conditions, all of them are required.")] public ConditionParameter[] conditions = new ConditionParameter[1];

        public DefaultTextureSettingsOverride settingsOverrides;

        public void Reset()
        {
            conditions = new ConditionParameter[1];
            settingsOverrides = new DefaultTextureSettingsOverride();
        }

        public bool IsDefault()
        {
            if (conditions.Length != 1 ||
                !string.IsNullOrEmpty(conditions[0].text) ||
                !settingsOverrides.IsDefault())
                return false;

            return true;
        }

        public bool AllConditionsValid(Object obj)
        {
            foreach(ConditionParameter c in conditions)
            {
                if (!IsConditionValid(c, obj))
                    return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public class DefaultTextureSettingsOverride
    {
        public TextureSettings textureParameters;

        [Space]
        public TexturePlatformSettings defaultPlatformSettings;
        [Space]
        public TexturePlatformSettings standaloneSettings;
        public TexturePlatformSettings iOSSettings;
        public TexturePlatformSettings androidSettings;
        public TexturePlatformSettings tvOSSettings;

        public bool IsDefault()
        {
            if (!textureParameters.IsDefault() ||
                defaultPlatformSettings.overrideSettings ||
                standaloneSettings.overrideSettings ||
                iOSSettings.overrideSettings ||
                androidSettings.overrideSettings)
                return false;

            return true;
        }
    }

    [System.Serializable]
    public class TextureSettings
    {
        [Space]
        public bool overrideTextureType = false;
        public TextureType textureType;
        public enum TextureType { Default, Sprite };

        [Space]
        public bool overrideGenerateMipMaps = false;
        public bool generateMipMaps = true;

        [Space]
        public bool overrideWrapMode = false;
        public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        public bool overrideFilterMode = false;
        public FilterMode filterMode = FilterMode.Bilinear;
        public bool overrideAnisoLevel = false;
        [Range(0, 16)] public int anisoLevel = 2;

        public bool IsDefault()
        {
            if (overrideTextureType ||
                overrideGenerateMipMaps ||
                overrideWrapMode ||
                overrideFilterMode ||
                overrideAnisoLevel)
                return false;

            return true;
        }
    }

    [System.Serializable]
    public class TexturePlatformSettings
    {
        public bool overrideSettings = false;

        public MaxSize maxSize = MaxSize._2048;
        public enum MaxSize { _32 = 32, _64 = 64, _128 = 128, _256 = 256, _512 = 512, _1024 = 1024, _2048 = 2048, _4096 = 4096, _8192 = 8192 };
        public TextureCompression textureCompression = TextureCompression.Compressed;
        public enum TextureCompression { Compressed, Uncompressed };
        [Range(0, 100)] public int compressorQuality = 50;
    }

    public enum Platforms
    {
        Default,
        Standalone,
        iPhone,
        Android,
        tvOS
    }

    #region PROPERTY DRAWERS

    [CustomPropertyDrawer(typeof(TextureSettings))]
    public class TextureSettingsDrawer : PropertyDrawer
    {
        private Rect position;
        private float height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Cache
            this.position = position;

            // Texture type
            float textureTypeHeight = position.y;
            OverrideField(position, property.FindPropertyRelative("textureType"), property.FindPropertyRelative("overrideTextureType"), textureTypeHeight);

            // Mip maps
            float mipMapsHeight = NextLine(textureTypeHeight, true);
            OverrideField(position, property.FindPropertyRelative("generateMipMaps"), property.FindPropertyRelative("overrideGenerateMipMaps"), mipMapsHeight);

            // Wrap mode
            float wrapModeHeight = NextLine(mipMapsHeight, true);
            OverrideField(position, property.FindPropertyRelative("wrapMode"), property.FindPropertyRelative("overrideWrapMode"), wrapModeHeight);

            // Filter mode
            float filterModeHeight = NextLine(wrapModeHeight);
            OverrideField(position, property.FindPropertyRelative("filterMode"), property.FindPropertyRelative("overrideFilterMode"), filterModeHeight);

            // Aniso level
            float anisoLevelHeight = NextLine(filterModeHeight);
            OverrideField(position, property.FindPropertyRelative("anisoLevel"), property.FindPropertyRelative("overrideAnisoLevel"), anisoLevelHeight);

            height = anisoLevelHeight + EditorGUIUtility.singleLineHeight - position.y;
        }
    }

    [CustomPropertyDrawer(typeof(TexturePlatformSettings))]
    public class TexturePlatformSettingsDrawer : PropertyDrawer
    {
        private int boxSpacing = 4;
        int showHideLabelWidth = 60;
        private bool compressedTexture;
        private float height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight + boxSpacing * 2;

            return (compressedTexture ? EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3 : EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2) + spacing + boxSpacing * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Box
            Rect innerPosition = EnhancedEditorGUI.Box(position, 1, boxSpacing);

            // Override button
            Rect buttonRect = new Rect(innerPosition.x + innerPosition.width - overrideButtonLength, innerPosition.y, overrideButtonLength, EditorGUIUtility.singleLineHeight);
            SerializedProperty overrideBool = property.FindPropertyRelative("overrideSettings");
            GUI.color = overrideBool.boolValue ? Color.green : Color.white;
            if (GUI.Button(buttonRect, overrideBool.boolValue ? "Override" : "Dismiss"))
            {
                overrideBool.boolValue = !overrideBool.boolValue;
            }

            // GUI
            GUI.color = Color.white;
            Rect labelRect = innerPosition.SetHeight(EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, $"<b>{property.displayName}</b>", EnhancedGUI.richText);

            if (property.isExpanded)
            {
                GUI.color = overrideBool.boolValue ? Color.white : Color.white.Alpha(0.75f);
                GUI.enabled = overrideBool.boolValue;

                Rect rect1 = labelRect.YOffset(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + spacing);
                EditorGUI.PropertyField(rect1, property.FindPropertyRelative("maxSize"));

                Rect rect2 = rect1.YOffset(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                SerializedProperty textureCompression = property.FindPropertyRelative("textureCompression");
                EditorGUI.PropertyField(rect2, textureCompression);

                compressedTexture = textureCompression.enumValueIndex == 0;

                if (compressedTexture)
                {
                    Rect rect3 = rect2.YOffset(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    EditorGUI.PropertyField(rect3, property.FindPropertyRelative("compressorQuality"));

                    height = rect3.y + EditorGUIUtility.singleLineHeight + boxSpacing;
                }
                else
                {
                    height = rect2.y + EditorGUIUtility.singleLineHeight + boxSpacing;
                }

                GUI.color = Color.white;
                GUI.enabled = true;
            }

            // Show / Hide button
            if (GUI.Button(new Rect(buttonRect.x - showHideLabelWidth - 8, buttonRect.y, showHideLabelWidth, EditorGUIUtility.singleLineHeight), property.isExpanded ? "Hide" : "Show"))
            {
                property.isExpanded = !property.isExpanded;
            }
        }
    }

    #endregion

    [CustomEditor(typeof(TextureSettingsOverride))]
    public class TextureSettingsOverrideEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TextureSettingsOverride settings = target as TextureSettingsOverride;

            EnhancedEditor.SmallSpace();

            GUI.enabled = !settings.IsDefault();

            EnhancedEditor.CenteredButton("Reset settings", settings.Reset);
        }
    }
}