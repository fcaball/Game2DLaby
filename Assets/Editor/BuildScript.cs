using System.IO;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    public static void BuildMazeGeneratorWithWebProfile()
    {
        // Charger le profil de build
        var bp = AssetDatabase.LoadAssetAtPath<BuildProfile>("Assets/Settings/Build Profiles/MazeGenerator-Web.asset");
        PlayerSettings.bundleVersion = IncrementBuildVersion(); // 🔹 Met à jour la version ici
        BuildPlayerWithProfileOptions options = new()
        {
            buildProfile = bp,
            options = BuildOptions.None,
        };


        // Lancer le build
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"✅ Build réussi : {summary.totalSize} bytes");
            string version = PlayerSettings.bundleVersion;
            File.WriteAllText("Builds/WEB/version.txt", version);
        }
        else
        {
            Debug.LogError("❌ Build échoué !");
        }

    }

    static string IncrementBuildVersion()
    {
        string currentVersion = PlayerSettings.bundleVersion;
        string[] parts = currentVersion.Split('.');

        if (parts.Length == 3 && int.TryParse(parts[2], out int patchVersion))
        {
            patchVersion++;
            string newVersion = $"{parts[0]}.{parts[1]}.{patchVersion}";
            return newVersion;
        }
        return currentVersion;
    }
}
