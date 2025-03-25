using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using System.Linq;

public class MazeData : MonoBehaviour
{
    // Liste statique qui va contenir les différentes données de chaque étage du labyrinthe
    public static List<FloorData> MazeFloors = new();
    public static int CurrentFloor;
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

                            TileInfos=new List<TileDatas>(){
                                 new(){
                                    Position=new(0,0,0),
                                    Rotation=new(0,0,0),
                                    Tag=0
                                }
                            }
                        }

            };

            MazeFloors = MazeFloors.Count == 0 ? new()
            {

                        new() {
                            Name = "Default",
                            EntreeSorties = new List<InOut>(),

                            TileInfos=new List<TileDatas>(){
                                 new(){
                                    Position=new(0,0,0),
                                    Rotation=new(0,0,0),
                                    Tag=0
                                }

                            }
                        }

            } : MazeFloors;
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

                            TileInfos=new List<TileDatas>(){
                                new(){
                                    Position=new(0,0,0),
                                    Rotation=new(0,0,0),
                                    Tag=0
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
    // private void OnApplicationQuit()
    // {
    //     SaveData();
    // }

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
    public List<TileDatas> TileInfos = new();
    public List<Vector3Int> Minerals=new();
    public string Name;
    public List<InOut> EntreeSorties = new();

    public bool ContainsTile(Vector3Int position)
    {
        return TileInfos.Any(tile => tile.Position == position);
    }

    public void AddOrReplaceTiles(List<TileDatas> tilesDatas)
    {
        Dictionary<Vector3Int, TileDatas> tileMap = TileInfos.ToDictionary(tile => tile.Position);

        foreach (var tileData in tilesDatas)
        {
            tileMap[tileData.Position] = tileData;
        }

        TileInfos = tileMap.Values.ToList();
    }
      public void AddOrReplaceTiles(HashSet<Vector3Int> newPositions)
    {
        Dictionary<Vector3Int, TileDatas> tileMap = TileInfos.ToDictionary(tile => tile.Position);

        foreach (var position in newPositions)
        {
            tileMap[position] = new TileDatas
            {
                Position = position,
                Rotation = new Vector3Int(0, 0, 0),
                Tag = 0
            };
        }

        TileInfos = tileMap.Values.ToList();
    }



}


[System.Serializable]
public class InOut
{
    public Vector3Int Location;
    public int IndexFloorDestination;
    public Vector3Int SpawnInDestination;

    public InOut(Vector3Int pos, int v,Vector3Int spawnDest)
    {
        Location = pos;
        IndexFloorDestination = v;
        SpawnInDestination = spawnDest;
    }
}

[System.Serializable]
public class TileDatas
{
    public TileTag Tag;
    public Vector3Int Position;
    public Vector3Int Rotation;
    
    public static Vector3Int GetTileRotation(TileTag tag)
    {

        switch (tag)
        {
            case TileTag.StraightVerticalWall:
                return new Vector3Int(0, 0, 90);
            case TileTag.CornerTopRightWall:
                return new Vector3Int(0, 0, -90);
            case TileTag.CornerDownRightWall:
                return new Vector3Int(0, 0, 180);
            case TileTag.CornerDownLeftWall:
                return new Vector3Int(0, 0, 90);
            case TileTag.ConnexionLeftTopRightWall:
                return new Vector3Int(0, 0, -90);
            case TileTag.ConnexionRightDownLeftWall:
                return new Vector3Int(0, 0, 90);
            case TileTag.ConnexionTopRightDownWall:
                return new Vector3Int(0, 0, 180);

            default: return new Vector3Int(0, 0, 0);
        }

    }

}

public enum TileTag
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

