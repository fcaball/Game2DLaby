using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;


public class BuildScript
{
    public static void BuildMazeGeneratorWithWindowsProfile()
    {
        // Mise à jour et sauvegarde de la version
        PlayerSettings.bundleVersion = IncrementBuildVersion();
        AssetDatabase.SaveAssets();

        // Définir le chemin de sortie
        string buildFolder = "MazeBuilds/Windows";
        string buildPath = $"{buildFolder}/MazeGenerator.exe"; // Exécutable Windows
        if (!Directory.Exists(buildFolder))
        {
            Directory.CreateDirectory(buildFolder);
        }

        string[] scenes = new string[] { "Assets/Scenes/MazeGenerator.unity" };

        // Lancer le build
        BuildReport report = BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"✅ Build réussi : {summary.totalSize} bytes");
            string version = PlayerSettings.bundleVersion;

            // Sauvegarder la version
            File.WriteAllText($"{buildFolder}/version.txt", version);
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
            return $"{parts[0]}.{parts[1]}.{patchVersion}";
        }
        return currentVersion;
    }
}
