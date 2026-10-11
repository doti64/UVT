using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using WebSocketSharp;
using Debug = UnityEngine.Debug;

public class uvtGitUtils : MonoBehaviour
{
    public static void CommitPush(string commitMsg)
    {
        if (!RunGit("add ."))
            return;

        if (!RunGit($"commit -m \"{commitMsg}\""))
            return;

        if (!RunGit("push"))
            return;
    }
    
    private static bool RunGit(string arguments)
    {
        var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = Application.dataPath + "/..",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Debug.LogError($"[VersionTool] Git failed:\n{error}");
            return false;
        }

        Debug.Log($"[VersionTool] Git: {output}");
        return true;
    }

    public static string commitMessage;
    public static void PushPage()
    {
        commitMessage = GUILayout.TextField(commitMessage);
        if (GUILayout.Button("Commit and Push"))
        {
            CommitPush(commitMessage);
        }
    }

    public static void PullPage()
    {
        if (GUILayout.Button("Pull"))
        {
            RunGit("pull");
        }
    }

    public static void InitalizeProjectTab()
    {
        if (GUILayout.Button("Initialize Git"))
        {
            RunGit("init");
            RunGit("add .");
            RunGit("commit -m \"Initial commit.\"");

            CreateGitIgnore();
        }
    }

    public static void CreateGitIgnore()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;

        string templatePath = Path.Combine(
            projectRoot, "Assets", "Editor", "UVT", "uvtGitIgnoreTemp.txt"
        );

        string destinationPath = Path.Combine(projectRoot, ".gitignore");

        if (!File.Exists(templatePath))
        {
            Debug.LogError("[VersionTool] gitignore template not found!");
            return;
        }

        if (File.Exists(destinationPath))
        {
            Debug.LogWarning("[VersionTool] .gitignore already exists. Skipping.");
            return;
        }

        string contents = File.ReadAllText(templatePath);
        File.WriteAllText(destinationPath, contents);

        Debug.Log("[VersionTool] created .gitignore!");
    }
    
    public static string githubUsername;
    public static string repositoryName;
    public static void ConnectToGithubTab()
    {
        GUILayout.Label("GitHub Username");
        githubUsername = GUILayout.TextField(githubUsername);
        GUILayout.Label("Repository Title");
        repositoryName = GUILayout.TextField(repositoryName);

        if (GUILayout.Button("Connect"))
        {
            if (githubUsername.IsNullOrEmpty() || repositoryName.IsNullOrEmpty())
            {
                Debug.LogError($"[VersionTool] GitHub username or the repository title cannot be empty.");
            }
            else
            {
                RunGit($"remote add origin https://github.com/{githubUsername}/{repositoryName}.git");
                RunGit("branch -M main");
                RunGit("push -u origin main");
            }
        }
    }
}
