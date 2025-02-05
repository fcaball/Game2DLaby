using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

public class MazeData : MonoBehaviour
{
    // Liste statique qui va contenir les différentes données de chaque étage du labyrinthe
    public static List<FloorData> MazeFloors = new();
    [SerializeField] private string _fileNameWithExtension;

    public void Awake()
    {
        LoadData();
    }


    // Fonction pour charger les données depuis un fichier JSON
    public void LoadData()
    {

        string filePath = Path.Combine(Application.persistentDataPath, _fileNameWithExtension);

        if (File.Exists(filePath) && File.ReadAllText(filePath) != "")
        {
            string json = File.ReadAllText(filePath);
            // Désérialiser le JSON en une liste de FloorData
            MazeFloors = JsonUtility.FromJson<MazeDataWrapper>(json).Floors;
            Debug.Log("Données chargées avec succès.");
            MazeFloors ??= new()
            {

                        new() {
                            Name = "Default",
                            EntreeSorties = new List<InOut>(),
                            TileMap = new List<Vector3Int>(){
                                new(){
                                    x=0,
                                    y=0,
                                    z=(int) TileType.Floor
                                }
                            }
                        }

            };

            MazeFloors=MazeFloors.Count==0?  new()
            {

                        new() {
                            Name = "Default",
                            EntreeSorties = new List<InOut>(),
                            TileMap = new List<Vector3Int>(){
                                new(){
                                    x=0,
                                    y=0,
                                    z=(int) TileType.Floor
                                }
                            }
                        }

            }:MazeFloors;
        }
        else
        {
            Debug.LogWarning("Le fichier n'existe pas. Création d'un fichier par défaut.");

            // Créer un contenu par défaut
            MazeDataWrapper defaultData = new()
            {
                Floors = new List<FloorData>()
                    {
                        new() {
                            Name = "Default",
                            EntreeSorties = new List<InOut>(),
                            TileMap = new List<Vector3Int>(){
                                new(){
                                    x=0,
                                    y=0,
                                    z=(int) TileType.Floor
                                }
                            }
                        }
                    }
            };

            MazeFloors.Add(defaultData.Floors[0]);

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
        string filePath = Path.Combine(Application.persistentDataPath, _fileNameWithExtension);
        MazeDataWrapper dataWrapper = new MazeDataWrapper();
        dataWrapper.Floors = MazeFloors;

        string json = JsonUtility.ToJson(dataWrapper, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Données sauvegardées avec succès.");
    }

    // Classe wrapper pour faciliter la sérialisation/désérialisation de la liste de FloorData
    [System.Serializable]
    public class MazeDataWrapper
    {
        public List<FloorData> Floors = new();
    }
}

[System.Serializable]
public class FloorData
{
    public List<Vector3Int> TileMap = new();
    public string Name;
    public List<InOut> EntreeSorties = new();

}


[System.Serializable]
public class InOut
{
    public List<TileData> Locations = new();
    public int IndexFloorDestination;
}

public enum TileType
{
    Floor = 0,
    StraightVerticalWall = 1,
    StraightHorizontalWall = 2,
    CornerTopLeftWall = 3,
    CornerDownLeftWall = 4,
    CornerTopRightWall = 5,
    CornerDownRightWall = 6,
    ConnexionLeftTopRightWall = 7,
    ConnexionRightDownLeftWall = 8,
    ConnexionTopLeftDownWall = 9,
    ConnexionTopRightDownWall = 10,
    ConnexionAllDirectionsWall = 11,


}

