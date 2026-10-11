using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class UVT : EditorWindow
{
    [MenuItem("Tools/UVT")]
    public static void ShowWindow()
    {
        UVT window = GetWindow<UVT>();
        window.titleContent = new GUIContent("Unity Version Tool");
        window.maxSize = new Vector2(800, 400);
        window.Show();
    }

    public int uiTab = 1;
    void OnGUI()
    {
        GUILayout.Label("UVT");
        if (uiTab != 1)
        {
            if (GUILayout.Button("Back"))
            {
                uiTab = 1;
            }
        }
        
        if (uiTab == 1) // Main
        {
            GUILayout.Label("Manage your version control here.");
            
            TabButton("Version Utility", 2);
            TabButton("Push Version", 3);
            TabButton("Pull Project", 4);
            TabButton("Initialize Project", 5);
            TabButton("Connect To GitHub", 6);
        }
        else if (uiTab == 2) // Game Version Tool
        {
            uvtGameVersionTool.VersionPage();
        }
        else if (uiTab == 3) // Push Version tab
        {
            uvtGitUtils.PushPage();
        }
        else if (uiTab == 4) // Pull tab
        {
            uvtGitUtils.PullPage();
        } 
        else if (uiTab == 5) // Init project tab
        {
            uvtGitUtils.InitalizeProjectTab();
        } 
        else if (uiTab == 6) // ok its self explanitory im done
        {
            uvtGitUtils.ConnectToGithubTab();
        } 
    }

    void TabButton(string buttonName, int uiValue)
    {
        if (GUILayout.Button(buttonName))
        {
            uiTab = uiValue;
        }
    }
}