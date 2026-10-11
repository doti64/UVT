using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class uvtGameVersionTool : MonoBehaviour, IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    
    private static (int major, int minor, int patch, int build) ParseVersion()
    {
        string versionString = PlayerSettings.bundleVersion.TrimStart('v', 'V');
        string[] digits = versionString.Split('.');

        int major = 0, minor = 0, patch = 0, build = 0;
        if (digits.Length >= 1) int.TryParse(digits[0], out major);
        if (digits.Length >= 2) int.TryParse(digits[1], out minor);
        if (digits.Length >= 3) int.TryParse(digits[2], out patch);
        if (digits.Length >= 4) int.TryParse(digits[3], out build);

        return (major, minor, patch, build);
    }

    public static void VersionPage()
    {
        var (major, minor, patch, build) = ParseVersion();
        
        GUILayout.Label("Current Game Version: " + PlayerSettings.bundleVersion);
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Bump Major (+1.0.0)"))
        {
            PlayerSettings.bundleVersion = $"v{major + 1}.0.0";
        }
        if (GUILayout.Button("Bump Minor (0.+1.0)"))
        {
            PlayerSettings.bundleVersion = $"v{major}.{minor + 1}.0";
        }
        if (GUILayout.Button("Bump Patch (0.0.+1)"))
        {
            PlayerSettings.bundleVersion = $"v{major}.{minor}.{patch + 1}";
        }
        GUILayout.EndHorizontal();
    }
    
    public string releaseVersion;
    public void OnPreprocessBuild(BuildReport report)
    {
        var (major, minor, patch, build) = ParseVersion();
        
        releaseVersion = $"{major}.{minor}.{patch}";
        string lastReleaseVersion = EditorPrefs.GetString("lastAndroidReleaseVersion", "");

        if (releaseVersion == lastReleaseVersion)
        {
            UVT.ShowWindow();

            throw new BuildFailedException(
                $"Version {releaseVersion} is unchanged from the last Android build. " +
                "Update the major, minor, or patch version before building.");
        }

        build++;
        PlayerSettings.Android.bundleVersionCode++;
        
        PlayerSettings.bundleVersion = $"v{releaseVersion}";
        //EditorPrefs.SetString("lastAndroidReleaseVersion", releaseVersion);
        
        Debug.Log($"[VersionTool] Incremented build number to {build}, new version compiling: {PlayerSettings.bundleVersion}");
    }
    
    public void OnPostprocessBuild(BuildReport report)
    {
        uvtGitUtils.CommitPush($"Automatic commit for build {PlayerSettings.bundleVersion}");
        
        Debug.Log("[VersionTool] Build committed and pushed to Git.");
        EditorPrefs.SetString("lastAndroidReleaseVersion", releaseVersion);
    }
}
