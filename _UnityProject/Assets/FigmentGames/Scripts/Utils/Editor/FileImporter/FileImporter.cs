using UnityEditor;
using UnityEngine;

namespace FigmentGames
{
    public class FileImporter : AssetPostprocessor
    {
        public const string iconsPath = "Assets/FigmentGames/Textures/Icons";

        private string[] platforms = new string[]
        {
            "Standalone",
            "Web",
            "iPhone",
            "Android",
            "WebGL",
            "Windows Store Apps",
            "PS4",
            "XboxOne",
            "Nintendo3DS",
            "tvOS"
        };

        #region MODEL SETTINGS

        private void OnPreprocessModel()
        {
            if (assetPath.Substring(assetPath.Length - 4, 4) == ".fbx") // Model is an FBX file
            {
                ModelImporter importer = assetImporter as ModelImporter;

                if (!importer)
                    return;

                // Model settings overrides
                FileImporterParameters fip = FileImporterParameters.Instance;
                if (!fip)
                    return;
                Object obj = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Object));
                bool hasOverride = false;

                for (int i = 0; i < fip.conditionalModelOverrides.Length; i++)
                {
                    // Missing TextureSettingsOverride
                    if (!fip.conditionalModelOverrides[i])
                        continue;

                    if (fip.conditionalModelOverrides[i].AllConditionsValid(obj))
                    {
                        OverrideModelSettings(importer, fip.conditionalModelOverrides[i].settingsOverrides);
                        hasOverride = true;

                        break;
                    }
                }

                // Default texture settings overrides
                if (hasOverride)
                    return;

                OverrideModelSettings(importer, fip.defaultModelOverrides);
            }
        }

        private void OverrideModelSettings(ModelImporter importer, DefaultModelSettingsOverride settings)
        {
            // Animation settings
            if (settings.animationSettings.overrideImportAnimation)
                importer.importAnimation = settings.animationSettings.importAnimation;

            // Materials settings
            if (settings.materialsSettings.overrideImportMaterials)
                importer.importMaterials = settings.materialsSettings.importMaterials;
        }

        #endregion

        #region TEXTURE SETTINGS

        private void OnPreprocessTexture()
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            if (!importer)
                return;

            // FigmentGames textures
            if (assetPath.Contains(iconsPath))
            {
                importer.mipmapEnabled = false;
                importer.crunchedCompression = false;
                importer.isReadable = true;
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 128;

                foreach (string platform in platforms)
                {
                    TextureImporterPlatformSettings tips = importer.GetPlatformTextureSettings(platform);
                    tips.overridden = false;
                    importer.SetPlatformTextureSettings(tips);
                }

                return;
            }

            // Texture settings overrides
            FileImporterParameters fip = FileImporterParameters.Instance;
            if (!fip)
                return;
            Object obj = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Object)) as Object;
            bool hasOverride = false;

            for (int i = 0; i < fip.conditionalTextureOverrides.Length; i++)
            {
                // Missing TextureSettingsOverride
                if (!fip.conditionalTextureOverrides[i])
                    continue;

                if (fip.conditionalTextureOverrides[i].AllConditionsValid(obj))
                {
                    OverrideTextureSettings(importer, fip.conditionalTextureOverrides[i].settingsOverrides);
                    hasOverride = true;

                    break;
                }
            }

            // Default texture settings overrides
            if (hasOverride)
                return;

            OverrideTextureSettings(importer, fip.defaultTextureOverrides);
        }

        private void OverrideTextureSettings(TextureImporter importer, DefaultTextureSettingsOverride settings)
        {
            // Texture type
            if (settings.textureParameters.overrideTextureType)
                importer.textureType = settings.textureParameters.textureType == TextureSettings.TextureType.Default ? TextureImporterType.Default : TextureImporterType.Sprite;

            // Mip maps
            if (settings.textureParameters.overrideGenerateMipMaps)
                importer.mipmapEnabled = settings.textureParameters.generateMipMaps;

            // Wrap mode
            if (settings.textureParameters.overrideWrapMode)
                importer.wrapMode = settings.textureParameters.wrapMode;

            // Filter mode
            if (settings.textureParameters.overrideFilterMode)
            {
                importer.filterMode = settings.textureParameters.filterMode;

                // Hard-coded fade behaviour
                importer.mipmapFadeDistanceStart = 5;
                importer.mipmapFadeDistanceEnd = 10;
            }

            // Aniso level
            if (settings.textureParameters.overrideAnisoLevel)
                importer.anisoLevel = settings.textureParameters.anisoLevel;

            // Platforms settings
            OverrideTexturePlatformSettings(importer, Platforms.Default, settings.defaultPlatformSettings);
            OverrideTexturePlatformSettings(importer, Platforms.Standalone, settings.standaloneSettings);
            OverrideTexturePlatformSettings(importer, Platforms.iPhone, settings.iOSSettings);
            OverrideTexturePlatformSettings(importer, Platforms.Android, settings.androidSettings);
            OverrideTexturePlatformSettings(importer, Platforms.tvOS, settings.tvOSSettings);
        }

        private void OverrideTexturePlatformSettings(TextureImporter importer, Platforms platform, TexturePlatformSettings settings)
        {
            // Cache
            bool compressed = settings.textureCompression == TexturePlatformSettings.TextureCompression.Compressed;

            // Default settings
            if (platform == Platforms.Default)
            {
                if (!settings.overrideSettings)
                    return;

                importer.maxTextureSize = (int)settings.maxSize;

                importer.crunchedCompression = compressed;
                importer.textureCompression = compressed ? TextureImporterCompression.Compressed : TextureImporterCompression.Uncompressed;
                importer.compressionQuality = settings.compressorQuality;

            }
            else
            {
                // Get tips
                TextureImporterPlatformSettings tips = importer.GetPlatformTextureSettings(platform.ToString());

                // Edit tips settings
                tips.overridden = settings.overrideSettings;

                if (settings.overrideSettings)
                {
                    tips.maxTextureSize = (int)settings.maxSize;

                    bool alpha = importer.DoesSourceTextureHaveAlpha();
                    bool isStandalone = platform == Platforms.Standalone;

                    tips.format = compressed ?
                        isStandalone ?
                        (alpha ? TextureImporterFormat.DXT5Crunched : TextureImporterFormat.DXT1Crunched) :
                        (alpha ? TextureImporterFormat.ETC2_RGBA8Crunched : TextureImporterFormat.ETC_RGB4Crunched) :
                    (alpha ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGB24);
                    
                }

                // Apply
                importer.SetPlatformTextureSettings(tips);
            }
        }

        #endregion
    }
}