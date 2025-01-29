using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

public class MazeData : MonoBehaviour
{
    // Liste statique qui va contenir les différentes données de chaque étage du labyrinthe
    public static List<FloorData> mazeFloors = new();
    [SerializeField] private string fileNameWithExtension;

    public void Awake()
    {
        LoadData();
    }


    // Fonction pour charger les données depuis un fichier JSON
    public void LoadData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileNameWithExtension);

        if (File.Exists(filePath) && File.ReadAllText(filePath)!="")
        {
            string json = File.ReadAllText(filePath);
            // Désérialiser le JSON en une liste de FloorData
            mazeFloors = JsonUtility.FromJson<MazeDataWrapper>(json).floors;
            Debug.Log("Données chargées avec succès.");
            if (mazeFloors == null)
            {
                mazeFloors = new();
            }
        }
        else
        {
            Debug.LogWarning("Le fichier n'existe pas. Création d'un fichier par défaut.");

            // Créer un contenu par défaut
            MazeDataWrapper defaultData = new()
            {
                floors = new List<FloorData>()
                    {
                        new() {
                            name = "Default",
                            entreeSorties = new List<InOut>(),
                            tileMap = new List<Vector3Int>(){
                                new(){
                                    x=0,
                                    y=0,
                                    z=(int) TileType.Floor
                                }
                            }
                        }
                    }
            };

            mazeFloors.Add(defaultData.floors[0]);

            // Sérialiser l'objet en JSON
            string defaultJson = JsonUtility.ToJson(defaultData, true);
            Debug.Log(defaultJson);

            // Écrire le fichier avec le contenu par défaut
            File.WriteAllText(filePath, defaultJson);
        }

    }
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // Fonction pour sauvegarder les données dans un fichier JSON
    public void SaveData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileNameWithExtension);
        MazeDataWrapper dataWrapper = new MazeDataWrapper();
        dataWrapper.floors = mazeFloors;

        string json = JsonUtility.ToJson(dataWrapper, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Données sauvegardées avec succès.");
    }

    // Classe wrapper pour faciliter la sérialisation/désérialisation de la liste de FloorData
    [System.Serializable]
    public class MazeDataWrapper
    {
        public List<FloorData> floors = new();
    }
}

[System.Serializable]
public class FloorData
{
    public List<Vector3Int> tileMap = new();
    public string name;
    public List<InOut> entreeSorties = new();

}


[System.Serializable]
public class InOut
{
    public List<TileData> locations = new();
    public int indexFloorDestination;
}

public enum TileType
{
    Floor=0,
    StraightVerticalWall=1,
    StraightHorizontalWall=2,
    CornerTopLeftWall=3,
    CornerDownLeftWall=4,
    CornerTopRightWall=5,
    CornerDownRightWall=6,
    ConnexionLeftTopRightWall=7,
    ConnexionRightDownLeftWall=8,
    ConnexionTopLeftDownWall=9,
    ConnexionTopRightDownWall=10,
    ConnexionAllDirectionsWall=11,

    
}
 
