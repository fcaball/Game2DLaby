using System.Diagnostics;
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
        BuildProfile.SetActiveBuildProfile(bp);
        BuildPlayerWithProfileOptions options = new ()
        {
            buildProfile = bp,
            locationPathName = "Builds/WEB/",
            options = BuildOptions.None
        };

        // Lancer le build
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log($"✅ Build réussi : {summary.totalSize} bytes");

            // Ajouter le build à Git et le pousser sur la branche "builds"
            CommitAndPushBuild();
        }
        else
        {
            UnityEngine.Debug.LogError("❌ Build échoué !");
        }
    }

   private static void CommitAndPushBuild()
{
    RunGitCommand("git fetch origin");  // Met à jour la liste des branches
    RunGitCommand("git checkout builds || git checkout -b builds");  // Bascule sur builds ou la crée si elle n'existe pas
    RunGitCommand("git pull origin builds");  // Récupère les derniers changements
    RunGitCommand("git add Builds/WEB");  // Ajoute le nouveau build
    RunGitCommand("git commit -m \"Mise à jour du build WebGL\"");  // Nouveau commit
    RunGitCommand("git push origin builds");  // Pousse le commit sans écraser
    UnityEngine.Debug.Log("📌 Build ajouté à la branche 'builds' !");
}

    private static void RunGitCommand(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = Process.Start(psi);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        
        if (!string.IsNullOrEmpty(output)) UnityEngine.Debug.Log(output);
        if (!string.IsNullOrEmpty(error)) UnityEngine.Debug.LogError(error);
    }
}
