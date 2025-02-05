using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class VersionChecker : MonoBehaviour
{
    private string _url = "https://fabiencaballero.fr/MazeGenerator/GetCurrentVersionNumber.php";
    private string _currentVersion;

    void Awake()
    {
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
            string versionNumber = request.downloadHandler.text;
            Debug.Log("Current Version Number from Server: " + versionNumber);
            CheckVersion(versionNumber);
        }
    }

    void CheckVersion(string versionNumber)
    {
        if (versionNumber == _currentVersion)
        {
            Debug.Log("The version is up to date.");
        }
        else
        {
            Debug.Log("A new version is available: " + versionNumber);
        }
    }
}
