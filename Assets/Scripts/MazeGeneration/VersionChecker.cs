using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class VersionChecker : MonoBehaviour
{
    private string _url = "https://fabiencaballero.fr/MazeGenerator/version.php";
    private string _currentVersion;
    [SerializeField] private TMPro.TMP_Text _textInfo;
    [SerializeField] private GameObject _updateRequest;
    [SerializeField] private PanelManager _panelManager;

    // 📌 Emplacement du ZIP téléchargé
    private string _zipFilePath;

    // 📌 Dossier actuel de l'application
    private string _appDirectory;


    void Awake()
    {
        _zipFilePath = Path.Combine(Application.persistentDataPath, "MazeBuild.zip");

        // 📌 Détecter le dossier réel de l'application
        _appDirectory = Directory.GetParent(Application.dataPath)?.FullName;
        _currentVersion = Application.version;
        Debug.Log("Current Application Version: " + _currentVersion);
        StartCoroutine(GetVersionNumber());
    }

    IEnumerator GetVersionNumber()
    {
        UnityWebRequest request = UnityWebRequest.Get(_url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string versionNumber =  JsonUtility.FromJson<VersionWrapper>(request.downloadHandler.text).Version;
            Debug.Log("Current Version Number from Server: " + versionNumber);
            CheckVersion(versionNumber);
        }
    }
    // Classe interne pour éviter une classe externe
    [System.Serializable]
    private class VersionWrapper
    {
        public string Version;
    }

    void CheckVersion(string versionNumber)
    {
        if (versionNumber.Equals(_currentVersion))
        {
            Debug.Log("The version is up to date.");
        }
        else
        {
            _panelManager.DisplayPanel(_updateRequest);
        }
    }

    public async void DownloadAndUpdate()
    {
        try
        {
            await DownloadFileAsync();
            ExtractAndReplaceFiles();
        }
        catch (Exception ex)
        {
            Debug.LogError($"⚠️ Erreur : {ex.Message}");
        }
    }


    private async Task DownloadFileAsync()
    {
        _textInfo.text = "Téléchargement en cours..."; // 📥 en Unicode

        using WebClient client = new WebClient();
        await client.DownloadFileTaskAsync(new Uri("https://fabiencaballero.fr/MazeGenerator/Windows/MazeBuild.zip"), _zipFilePath);

        _textInfo.text = ($"✅ Téléchargement terminé : {_zipFilePath}");
    }

    private async Task ExtractAndReplaceFiles()
    {
        _textInfo.text = ("Extraction des fichiers...");

        string extractPath = Path.Combine(Application.persistentDataPath, "MazeBuild");

        if (Directory.Exists(extractPath))
        {
            Directory.Delete(extractPath, true);
        }

        ZipFile.ExtractToDirectory(_zipFilePath, extractPath);
        _textInfo.text = ("Extraction terminée");


        _textInfo.text = ("Mise à jour terminée ! L'application va redémarrer");
        await Task.Delay(3000);
        RestartApplication(extractPath);
    }

    private void RestartApplication(string sourcePath)
    {
        string batchFilePath = Path.Combine(sourcePath, "update.bat");

        // Créer le script batch
        string batchScript = $@"
@echo off
timeout /t 5 >nul

:: ✅ Supprimer les anciens fichiers
rmdir /s /q ""{_appDirectory}"" >nul 2>&1
mkdir ""{_appDirectory}""

:: ✅ Copier les nouveaux fichiers
xcopy /E /C /H /Y ""{sourcePath}\*"" ""{_appDirectory}\"" >nul 2>&1

:: 🚀 Lancer la nouvelle version
""{_appDirectory}\MazeGenerator.exe""

:: Supprimer ce script après exécution
del ""%~f0""
";

        // Écrire le fichier batch sur le disque
        File.WriteAllText(batchFilePath, batchScript);

        // Exécuter le script batch
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = batchFilePath,
            UseShellExecute = true,  // Important pour exécuter un fichier .bat
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);

        // Fermer l'application Unity
        Application.Quit();
    }
}
