using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildScript
{
    static readonly string[] Scenes =
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/StructureDisplay.unity",
        "Assets/Scenes/PrincipleDemo.unity",
        "Assets/Scenes/OperationTraining.unity",
        "Assets/Scenes/ScoreManagement.unity",
    };

    public static void BuildWindows64()
    {
        string outputDir = Environment.GetEnvironmentVariable("BUILD_PATH")
                           ?? Environment.GetEnvironmentVariable("BUILD_OUTPUT_DIR")
                           ?? "Builds/Windows";
        string exe = Path.Combine(outputDir, "VirtualSimSystem.exe");

        Directory.CreateDirectory(outputDir);

        // Addressables bundle 需在有素材时手动执行：
        // Window > Asset Management > Addressables > Groups > Build > New Build
        Debug.LogWarning("[Build] Addressables not built in CI (no assets). Run manually before release.");

        var options = new BuildPlayerOptions
        {
            scenes           = Scenes,
            locationPathName = exe,
            target           = BuildTarget.StandaloneWindows64,
            options          = BuildOptions.None,
        };

        var report  = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[Build] OK → {exe}  ({summary.totalSize / 1024 / 1024} MB)");
        }
        else
        {
            Debug.LogError($"[Build] FAILED: {summary.result} — errors: {summary.totalErrors}");
            EditorApplication.Exit(1);
        }
    }
}
