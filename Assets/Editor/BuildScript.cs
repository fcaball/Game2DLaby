using System.IO;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    public static void BuildMazeGeneratorWithWindowsProfile()
{
    // Charger le profil de build
    // var bp = AssetDatabase.LoadAssetAtPath<BuildProfile>("Assets/Settings/Build Profiles/MazeGenerator-Web.asset");
    PlayerSettings.bundleVersion = IncrementBuildVersion(); // 🔹 Met à jour la version ici

    // Définir le chemin de sortie
    string buildPath = "MazeBuilds/Windows"; // Le répertoire où le build sera sauvegardé
    if (!Directory.Exists(buildPath))
    {
        Directory.CreateDirectory(buildPath); // Créer le répertoire s'il n'existe pas
    }

    // BuildPlayerWithProfileOptions options = new()
    // {
    //     buildProfile = bp,
    //     options = BuildOptions.None,
    // };

    string[] scenes = new string[] { "Assets/Scenes/MazeGenerator.unity" }; // Exemple de scène

    // Lancer le build
    BuildReport report = BuildPipeline.BuildPlayer(scenes,buildPath,BuildTarget.StandaloneWindows,BuildOptions.None);
    BuildSummary summary = report.summary;

    if (summary.result == BuildResult.Succeeded)
    {
        Debug.Log($"✅ Build réussi : {summary.totalSize} bytes");
        string version = PlayerSettings.bundleVersion;

        // Sauvegarder la version dans un fichier texte dans le dossier de build
        File.WriteAllText($"{buildPath}/version.txt", version);

        // Assurer que l'artifact est dans le dossier correct
        string artifactPath = $"{buildPath}/index.html"; // Exemple de fichier généré pour WebGL
        Debug.Log($"Build créé à : {artifactPath}");
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
