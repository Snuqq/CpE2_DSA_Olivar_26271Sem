using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SetupSearchScene
{
    [MenuItem("Search Demo/Create scene")]
    public static void Create()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("Search demonstration").AddComponent<SearchStage>();
        EditorSceneManager.SaveScene(scene, "Assets/Search.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Search.unity", true) };
        AssetDatabase.SaveAssets();
    }

    public static void BuildWindows()
    {
        if (!System.IO.File.Exists("Assets/Search.unity")) throw new System.IO.FileNotFoundException("Create the search scene before building.");
        SearchMotionChecks.Run();
        PlayerSettings.runInBackground = true;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.allowFullscreenSwitch = false;
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/Search.unity" },
            locationPathName = "Build/FurinaSearch.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new System.Exception("Unity Windows build failed: " + report.summary.result);
    }

    public static void Preview()
    {
        EditorSceneManager.OpenScene("Assets/Search.unity", OpenSceneMode.Single);
        var demo = Object.FindFirstObjectByType<SearchStage>();
        demo.Initialize();
        SearchPerformanceChecks.Measure(demo);
        var camera = Object.FindFirstObjectByType<Camera>();
        var target = new RenderTexture(1100, 700, 24);
        camera.targetTexture = target;
        camera.Render();
        RenderTexture.active = target;
        var image = new Texture2D(1100, 700, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1100, 700), 0, 0);
        image.Apply();
        System.IO.File.WriteAllBytes("../obj/unity-stage-preview.png", image.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(image);
        Object.DestroyImmediate(target);
    }
}
