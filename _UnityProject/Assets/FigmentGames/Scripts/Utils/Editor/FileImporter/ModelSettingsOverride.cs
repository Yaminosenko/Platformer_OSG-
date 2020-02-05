using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    using static FileImporterParameters;

    [CreateAssetMenu(fileName = "ModelSettingsOverride", menuName = "Figment Games/File Importer/Model Settings Override")]
    public class ModelSettingsOverride : ScriptableObject
    {
        [Space]
        [Tooltip("When using multiple conditions, all of them are required.")] public ConditionParameter[] conditions = new ConditionParameter[1];

        public DefaultModelSettingsOverride settingsOverrides;

        public void Reset()
        {
            conditions = new ConditionParameter[1];
            settingsOverrides.Reset();
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
            foreach (ConditionParameter c in conditions)
            {
                if (!IsConditionValid(c, obj))
                    return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public class DefaultModelSettingsOverride
    {
        //public ModelSettings modelSettings;
        //public RigSettings rigSettings;
        public AnimationSettings animationSettings;
        public MaterialsSettings materialsSettings;

        public void Reset()
        {
            //modelSettings = new ModelSettings();
            //rigSettings = new RigSettings();
            animationSettings = new AnimationSettings();
            materialsSettings = new MaterialsSettings();
        }

        public bool IsDefault()
        {
            if (//!modelSettings.IsDefault() ||
                //!rigSettings.IsDefault() ||
                !animationSettings.IsDefault() ||
                !materialsSettings.IsDefault())
                return false;

            return true;
        }
    }

    [System.Serializable]
    public class ModelSettings
    {
        public bool IsDefault()
        {
            return true;
        }
    }

    [System.Serializable]
    public class RigSettings
    {
        public bool IsDefault()
        {
            return true;
        }
    }

    [System.Serializable]
    public class AnimationSettings
    {
        public bool overrideImportAnimation;
        public bool importAnimation;

        public bool IsDefault()
        {
            if (overrideImportAnimation)
                return false;

            return true;
        }
    }

    [System.Serializable]
    public class MaterialsSettings
    {
        public bool overrideImportMaterials;
        public bool importMaterials;

        public bool IsDefault()
        {
            if (overrideImportMaterials)
                return false;

            return true;
        }
    }

    #region PROPERTY DRAWERS

    [CustomPropertyDrawer(typeof(AnimationSettings))]
    public class AnimationSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Import animation
            OverrideField(position, property.FindPropertyRelative("importAnimation"), property.FindPropertyRelative("overrideImportAnimation"));
        }
    }

    [CustomPropertyDrawer(typeof(MaterialsSettings))]
    public class MaterialsSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Import materials
            OverrideField(position, property.FindPropertyRelative("importMaterials"), property.FindPropertyRelative("overrideImportMaterials"));
        }
    }

    #endregion

    [CustomEditor(typeof(ModelSettingsOverride))]
    public class ModelSettingsOverrideEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ModelSettingsOverride settings = target as ModelSettingsOverride;

            EnhancedEditor.SmallSpace();

            GUI.enabled = !settings.IsDefault();

            EnhancedEditor.CenteredButton("Reset settings", settings.Reset);
        }
    }
}