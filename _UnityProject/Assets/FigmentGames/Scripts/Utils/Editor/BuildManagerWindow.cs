using UnityEngine;
using UnityEditor;
using System.IO;

namespace FigmentGames
{
    using System.Collections.Generic;
    using UnityEditor.Build.Reporting;
    using static EnhancedEditor;

    public class BuildManagerWindow : EditorWindow
    {
        private string buildName = "Build";
        private string buildLocation;
        private string defaultDefineSymbols;
        private string devDefineSymbols;
        private string storeDeploymentDefineSymbols;
        private bool isStoreBuild;
        private bool incrementGameVersion = true;
        private bool incrementBundleVersionCode = true;
        private string keystorePass;

        private const string keyBuildName = "buildName";
        private const string keyBuildlocation = "buildLocation";
        private const string keyDefaultDefineSymbols = "defaultDefineSymbols";
        private const string keyDevDefineSymbols = "devDefineSymbols";
        private const string keyStoreDeploymentDefineSymbols = "storeDeploymentDefineSymbols";
        private const string keyIsStoreBuild = "isStoreBuild";
        private const string keyIncrementGameVersion = "incrementGameVersion";
        private const string keyIncrementBundleVersionCode = "incrementBundleVersionCode";
        private const string keyKeystorePass = "keystorePass";


        [MenuItem("Figment Games/Build Manager %#&b")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(BuildManagerWindow), false, "Build Manager");
        }


        private void OnFocus()
        {
            buildName = EditorPrefs.GetString(keyBuildName, "BuildName");
            buildLocation = EditorPrefs.GetString(keyBuildlocation);

            defaultDefineSymbols = EditorPrefs.GetString(keyDefaultDefineSymbols);
            devDefineSymbols = EditorPrefs.GetString(keyDevDefineSymbols);
            storeDeploymentDefineSymbols = EditorPrefs.GetString(keyStoreDeploymentDefineSymbols);

            isStoreBuild = EditorPrefs.GetInt(keyIsStoreBuild, 0) == 1;

            incrementGameVersion = EditorPrefs.GetInt(keyIncrementGameVersion, 1) == 1;
            incrementBundleVersionCode = EditorPrefs.GetInt(keyIncrementBundleVersionCode, 1) == 1;

            keystorePass = EditorPrefs.GetString(keyKeystorePass);
        }

        private void OnGUI()
        {
            // Cache
            string appVersion;
            int bundleVersionCode;
            bool buildLocationExists = Directory.Exists(buildLocation);

            EditorGUI.BeginChangeCheck();
            {
                LargeSpace();

                // Build options
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("<color=orange>BUILD OPTIONS</color>".Bold(), EnhancedGUI.centeredWrapTextStyle);

                    buildName = EditorGUILayout.DelayedTextField("Build Name", buildName);
                    buildLocation = EditorGUILayout.DelayedTextField("Build Location", buildLocation);

                    if (!buildLocationExists)
                        GUILayout.Label("Build location does not exist.".Color(Color.red), EnhancedGUI.richText);

                    SmallSpace();

                    GUILayout.BeginHorizontal();
                    {
                        incrementGameVersion = GUILayout.Toggle(incrementGameVersion, "Increment Game Version");
                        GUILayout.Label(IncrementVersionString(PlayerSettings.bundleVersion, "Application version", incrementGameVersion, out appVersion), EnhancedGUI.richText);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        incrementBundleVersionCode = GUILayout.Toggle(incrementBundleVersionCode, "Increment Bundle Version Code");
                        GUILayout.Label(IncrementVersionString(PlayerSettings.Android.bundleVersionCode, "Bundle Version Code", incrementBundleVersionCode, out bundleVersionCode), EnhancedGUI.richText);
                    }
                    GUILayout.EndHorizontal();

                    keystorePass = EditorGUILayout.DelayedTextField("Keystore Password", keystorePass);
                }
                GUILayout.EndVertical();

                SmallSpace();

                // Define symbols
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("<color=orange>DEFINE SYMBOLS</color>".Bold(), EnhancedGUI.centeredWrapTextStyle);
                    defaultDefineSymbols = EditorGUILayout.DelayedTextField("Default", defaultDefineSymbols);
                    devDefineSymbols = EditorGUILayout.DelayedTextField("Dev", devDefineSymbols);
                    storeDeploymentDefineSymbols = EditorGUILayout.DelayedTextField("Store", storeDeploymentDefineSymbols);
                }
                GUILayout.EndVertical();

                SmallSpace();

                // Build buttons
                GUILayout.BeginHorizontal("box", GUILayout.Height(64));
                {
                    GUILayout.BeginVertical();
                    {
                        GUI.color = isStoreBuild ? Color.white : Color.green;
                        if (GUILayout.Button("Developer Build", GUILayout.ExpandHeight(true)))
                        {
                            isStoreBuild = false;
                        }

                        GUI.color = isStoreBuild ? Color.green : Color.white;
                        if (GUILayout.Button("Store Build", GUILayout.ExpandHeight(true)))
                        {
                            isStoreBuild = true;
                        }
                    }
                    GUILayout.EndVertical();

                    GUI.enabled = buildLocationExists;
                    GUI.color = buildLocationExists ? Color.yellow : Color.red;
                    if (GUILayout.Button("START BUILD".Bold(), EnhancedGUI.richButton, GUILayout.ExpandHeight(true)))
                    {
                        Build(appVersion, bundleVersionCode);
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);

                EditorPrefs.SetString(keyBuildName, buildName);
                EditorPrefs.SetString(keyBuildlocation, buildLocation);

                EditorPrefs.SetString(keyDefaultDefineSymbols, defaultDefineSymbols);
                EditorPrefs.SetString(keyDevDefineSymbols, devDefineSymbols);
                EditorPrefs.SetString(keyStoreDeploymentDefineSymbols, storeDeploymentDefineSymbols);

                EditorPrefs.SetInt(keyIsStoreBuild, isStoreBuild ? 1 : 0);

                EditorPrefs.SetInt(keyIncrementGameVersion, incrementGameVersion ? 1 : 0);
                EditorPrefs.SetInt(keyIncrementBundleVersionCode, incrementBundleVersionCode ? 1 : 0);

                EditorPrefs.SetString(keyKeystorePass, keystorePass);
            }
        }


        private string IncrementVersionString(string version, string descriptiveName, bool increment, out string newVersion)
        {
            int lastDotIndex = version.LastIndexOf(".");
            bool incrementable = int.TryParse(version.Substring(lastDotIndex + 1), out int end);
            newVersion = increment ? $"{version.Substring(0, lastDotIndex + 1)}{end + 1}" : version;
            string newVersionString = increment ? $"{version.Substring(0, lastDotIndex + 1)}{(end + 1).ToString().Color(Color.green)}" : version;
            return incrementable ? $"{version} ➜ {newVersionString}" : $"{descriptiveName} can't be read properly".Color(Color.red);
        }

        private string IncrementVersionString(int version, string descriptiveName, bool increment, out int newVersion)
        {
            newVersion = increment ? version + 1 : version;
            string newVersionString = increment ? $"{newVersion.ToString().Color(Color.green)}" : version.ToString();
            return $"{version} ➜ {newVersionString}";
        }

        private void Build(string appVersion, int bundleVersionCode)
        {
            // Cache
            string baseAppVersion = PlayerSettings.bundleVersion;
            int baseBundleVersionCode = PlayerSettings.Android.bundleVersionCode;

            // Build setup
#if UNITY_PRO_LICENSE
            PlayerSettings.SplashScreen.show = false;
#endif
            PlayerSettings.bundleVersion = appVersion;
            PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, $"{defaultDefineSymbols};{(isStoreBuild ? storeDeploymentDefineSymbols : devDefineSymbols)}");

            System.DateTime date = System.DateTime.Today;
            string outBuildName = $"{buildName}_{date.Year}_{date.Month.ToString("00")}_{date.Day.ToString("00")}";
            if (isStoreBuild)
                outBuildName += $"_Store";

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystorePass = PlayerSettings.Android.keyaliasPass = keystorePass;

            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;

            // Build
            EditorUserBuildSettings.development = false;

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            int sceneCount = EditorBuildSettings.scenes.Length;
            List<string> scenePaths = new List<string>(sceneCount);
            for (int i = 0; i < sceneCount; i++)
                scenePaths.Add(EditorBuildSettings.scenes[i].path);
            buildPlayerOptions.scenes = scenePaths.ToArray();

            buildPlayerOptions.locationPathName = $"{buildLocation}/{outBuildName}.apk";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            else if (summary.result == BuildResult.Failed)
                Debug.Log("Build failed");

            // Post build
            if (summary.result == BuildResult.Failed ||
                summary.result == BuildResult.Cancelled)
            {
                // Revert versions to pre-build ones
                PlayerSettings.bundleVersion = baseAppVersion;
                PlayerSettings.Android.bundleVersionCode = baseBundleVersionCode;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defaultDefineSymbols); // Reset define symbols
        }
    }
}