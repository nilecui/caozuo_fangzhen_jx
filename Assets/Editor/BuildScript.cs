using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
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
        // 输出路径优先读环境变量，方便 CI 覆盖
        string outputDir = Environment.GetEnvironmentVariable("BUILD_OUTPUT_DIR")
                           ?? "Builds/Windows";
        string exe = Path.Combine(outputDir, "VirtualSimSystem.exe");

        Directory.CreateDirectory(outputDir);

        BuildAddressables();

        var options = new BuildPlayerOptions
        {
            scenes            = Scenes,
            locationPathName  = exe,
            target            = BuildTarget.StandaloneWindows64,
            options           = BuildOptions.None,
        };

        var report = BuildPipeline.BuildPlayer(options);
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

    static void BuildAddressables()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogWarning("[Build] Addressable settings not found, skipping.");
            return;
        }
        AddressableAssetSettings.BuildPlayerContent(out var result);
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError($"[Build] Addressables failed: {result.Error}");
            EditorApplication.Exit(1);
        }
        Debug.Log("[Build] Addressables OK");
    }
}
