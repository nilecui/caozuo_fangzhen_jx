using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// One-click scene configurator. Run via menu: VirtualSim > Setup All Scenes
/// </summary>
public static class SceneSetup
{
    const string PanelSettingsPath = "Assets/UI/PanelSettings.asset";

    [MenuItem("VirtualSim/Setup All Scenes")]
    public static void SetupAllScenes()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Scene Setup", "Stop Play Mode first, then run Setup All Scenes.", "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog("Scene Setup",
            "This will overwrite all 5 scene files. Save any unsaved work first.\n\nContinue?",
            "Setup", "Cancel"))
            return;

        EnsurePanelSettings();
        SetupMainMenu();
        SetupStructureDisplay();
        SetupPrincipleDemo();
        SetupOperationTraining();
        SetupScoreManagement();
        ConfigureBuildSettings();

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Scene Setup", "All scenes configured successfully!\n\nOpen MainMenu.unity and press Play to test.", "OK");
        Debug.Log("[SceneSetup] Done. Open MainMenu.unity and press Play.");
    }

    // ------------------------------------------------------------------ helpers

    static PanelSettings EnsurePanelSettings()
    {
        var ps = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
        if (ps != null) return ps;

        ps = ScriptableObject.CreateInstance<PanelSettings>();
        ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        ps.referenceResolution = new Vector2Int(1920, 1080);
        AssetDatabase.CreateAsset(ps, PanelSettingsPath);
        AssetDatabase.SaveAssets();
        Debug.Log($"[SceneSetup] Created PanelSettings at {PanelSettingsPath}");
        return ps;
    }

    static UIDocument AddUIDocument(GameObject go, string uxmlPath)
    {
        var doc = go.AddComponent<UIDocument>();
        var ps  = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
        if (ps != null)
            doc.panelSettings = ps;
        else
            Debug.LogWarning("[SceneSetup] PanelSettings not found — UI will not render.");

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        if (uxml != null)
            doc.visualTreeAsset = uxml;
        else
            Debug.LogWarning($"[SceneSetup] UXML not found: {uxmlPath}");

        return doc;
    }

    static void ClearScene(UnityEngine.SceneManagement.Scene scene)
    {
        foreach (var go in scene.GetRootGameObjects())
            Object.DestroyImmediate(go);
    }

    static GameObject MakeCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.AddComponent<Camera>();
        go.AddComponent<AudioListener>();
        go.transform.position = new Vector3(0f, 1f, -10f);
        return go;
    }

    // ------------------------------------------------------------------ scenes

    static void SetupMainMenu()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        ClearScene(scene);

        MakeCamera();

        new GameObject("AppManager").AddComponent<AppManager>();
        new GameObject("AudioManager").AddComponent<AudioManager>();
        new GameObject("SceneLoader").AddComponent<SceneLoader>();

        var uiGo = new GameObject("UI");
        AddUIDocument(uiGo, "Assets/UI/UXML/MainMenu.uxml");
        uiGo.AddComponent<MainMenuController>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] MainMenu ✓");
    }

    static void SetupStructureDisplay()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/StructureDisplay.unity");
        ClearScene(scene);

        var camGo = MakeCamera();
        var orbit = camGo.AddComponent<ModelOrbitController>();

        var targetGo = new GameObject("OrbitTarget");
        orbit.target = targetGo.transform;

        var selectorGo  = new GameObject("PartSelector");
        var selector    = selectorGo.AddComponent<PartSelector>();
        selector.mainCamera = camGo.GetComponent<Camera>();

        var uiGo = new GameObject("UI");
        AddUIDocument(uiGo, "Assets/UI/UXML/MainLayout.uxml");
        var infoPanel = uiGo.AddComponent<InfoPanelController>();
        infoPanel.partSelector = selector;

        var presets = uiGo.AddComponent<CameraPresetController>();
        presets.orbitController = orbit;

        uiGo.AddComponent<ExplodeViewController>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] StructureDisplay ✓");
    }

    static void SetupPrincipleDemo()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/PrincipleDemo.unity");
        ClearScene(scene);

        MakeCamera();

        var uiGo = new GameObject("UI");
        AddUIDocument(uiGo, "Assets/UI/UXML/PrincipleDemo.uxml");
        uiGo.AddComponent<PrincipleDemoController>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] PrincipleDemo ✓");
    }

    static void SetupOperationTraining()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/OperationTraining.unity");
        ClearScene(scene);

        var camGo  = MakeCamera();
        var orbit  = camGo.AddComponent<ModelOrbitController>();

        var targetGo = new GameObject("OrbitTarget");
        orbit.target = targetGo.transform;

        var selectorGo = new GameObject("PartSelector");
        var selector   = selectorGo.AddComponent<PartSelector>();
        selector.mainCamera = camGo.GetComponent<Camera>();

        var uiGo = new GameObject("UI");
        AddUIDocument(uiGo, "Assets/UI/UXML/MainLayout.uxml");
        var progressBar = uiGo.AddComponent<StepProgressBarController>();
        var subtitle    = uiGo.AddComponent<SubtitleController>();
        var feedback    = uiGo.AddComponent<FeedbackOverlayController>();

        new GameObject("TrainingTimer").AddComponent<TrainingTimer>();

        var flowGo  = new GameObject("TrainingFlowController");
        var flow    = flowGo.AddComponent<TrainingFlowController>();
        flow.progressBar  = progressBar;
        flow.subtitle     = subtitle;
        flow.feedback     = feedback;
        flow.partSelector = selector;

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] OperationTraining ✓");
    }

    static void SetupScoreManagement()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/ScoreManagement.unity");
        ClearScene(scene);

        MakeCamera();

        var uiGo = new GameObject("UI");
        AddUIDocument(uiGo, "Assets/UI/UXML/ScoreReport.uxml");
        uiGo.AddComponent<ScoreReportController>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneSetup] ScoreManagement ✓");
    }

    // ------------------------------------------------------------------ build settings

    static void ConfigureBuildSettings()
    {
        var paths = new[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/StructureDisplay.unity",
            "Assets/Scenes/PrincipleDemo.unity",
            "Assets/Scenes/OperationTraining.unity",
            "Assets/Scenes/ScoreManagement.unity",
        };

        var entries = new EditorBuildSettingsScene[paths.Length];
        for (int i = 0; i < paths.Length; i++)
            entries[i] = new EditorBuildSettingsScene(paths[i], true);

        EditorBuildSettings.scenes = entries;
        Debug.Log("[SceneSetup] Build settings updated ✓");
    }
}
