using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Networking;


public class BuildScript
{
    public static async Task BuildMazeGeneratorWithWindowsProfile()
    {
        // Mise à jour et sauvegarde de la version
        PlayerSettings.bundleVersion = IncrementBuildVersion(await GetVersionFromServer());
        PlayerSettings.productName="MazeGenerator";
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

    static string IncrementBuildVersion(string currentVersion)
    {
        string[] parts = currentVersion.Split('.');

        if (parts.Length == 3 && int.TryParse(parts[2], out int patchVersion))
        {
            patchVersion++;
            return $"{parts[0]}.{parts[1]}.{patchVersion}";
        }
        return currentVersion;
    }

     public static async Task<string> GetVersionFromServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://fabiencaballero.fr/MazeGenerator/version.php"))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                string versionData = JsonUtility.FromJson<string>(json);
                Debug.Log($"version : {versionData}");
                return versionData;
            }
            else
            {
                Debug.LogError("❌ Erreur lors de la récupération de la version : " + request.error);
                return"0.0.0"; // Valeur par défaut en cas d'erreur
            }
        }
    }

}
