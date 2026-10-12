using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    [MenuItem("Ruins/Сборка/Windows")]
    public static void BuildWindows() =>
        Build(BuildTarget.StandaloneWindows64, "Builds/Windows/Ruins.exe");

    [MenuItem("Ruins/Сборка/macOS")]
    public static void BuildMac() =>
        Build(BuildTarget.StandaloneOSX, "Builds/macOS/Ruins.app");

    [MenuItem("Ruins/Сборка/Web")]
    public static void BuildWeb()
    {
        // GitHub Pages и многие хостинги не отдают заголовок
        // Content-Encoding — распаковку возьмёт на себя загрузчик
        PlayerSettings.WebGL.decompressionFallback = true;
        Build(BuildTarget.WebGL, "Builds/Web");
    }

    private static void Build(BuildTarget target, string path)
    {
        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = path,
            target = target,
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        Debug.Log($"{target}: {summary.result}, " +
                  $"{summary.totalSize / (1024 * 1024)} МБ, " +
                  $"{summary.totalTime.TotalSeconds:F0} с");

        // На сервере сборки ошибка должна вернуть ненулевой код
        if (Application.isBatchMode && summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
