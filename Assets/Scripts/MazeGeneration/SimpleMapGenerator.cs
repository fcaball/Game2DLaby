using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SimpleMapGenerator : MonoBehaviour
{
    [SerializeField] protected Vector3Int startPosition;
    private int iterations = 0;
    private int walkLength = 0;
    private float probability = 0.0f;
    private bool isRoomCreationIsPossible = true;
    private bool isEachIterationStartFromRandomlyPosition = true;
    [SerializeField] private TileMapVisualizer tileMapVisualizer;
    private HashSet<Vector3Int> mapTiles = new();
    [SerializeField] private UnityEvent MissingParameter;
    [SerializeField] private GameObject RunButton;
    [SerializeField] private Image LoadingPicture;
    private int currentAlgo;

    public void SetCurrentAlgo(int index)
    {
        currentAlgo = index;
    }

    #region General Run
    public void Run()
    {
        switch (currentAlgo)
        {
            case 0:
                if (iterations == 0 || walkLength == 0)
                {
                    MissingParameter.Invoke();
                }
                else
                {
                    StartCoroutine(RunProceduralGenerationCoroutine(RunRandomWalkCoroutine(startPosition,true)));
                }
                break;

            case 1:
                ClearWalls();
                StartCoroutine(RunProceduralGenerationCoroutine(RunWallGeneration()));
                break;
            case 2:
                if (iterations == 0 || walkLength == 0)
                {
                    MissingParameter.Invoke();
                }
                else
                {
                    StartCoroutine(RunProceduralGenerationCoroutine(RunCorridorWalkCoroutine()));
                }
                break;

        }
    }


    private IEnumerator RunProceduralGenerationCoroutine(IEnumerator enumerator)
    {
        RunButton.SetActive(false);
        LoadingPicture.gameObject.SetActive(true);

        yield return StartCoroutine(enumerator);

        tileMapVisualizer.PaintTiles();
        LoadingPicture.fillAmount = 0;
        LoadingPicture.gameObject.SetActive(false);
        RunButton.SetActive(true);
    }

    #endregion

    #region RandomWalk


    private IEnumerator RunRandomWalkCoroutine(Vector3Int startPos, bool isLoadingHaveToBeUpdated)
    {
        var currentPosition = startPos;
        HashSet<Vector3Int> positions = new();
        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, walkLength, mapTiles.ToList());

            positions.UnionWith(path);
            if (isEachIterationStartFromRandomlyPosition)
            {
                int index = Random.Range(0, positions.Count);
                while (positions.ElementAt(index).z == (int)TileType.StraightVerticalWall)
                {
                    index = Random.Range(0, positions.Count);
                }
                currentPosition = new Vector3Int(positions.ElementAt(index).x, positions.ElementAt(index).y, 0);
            }
            if (isLoadingHaveToBeUpdated)
                LoadingPicture.fillAmount = (float)i / iterations;
            yield return null;
        }
        mapTiles.UnionWith(positions);
    }
    #endregion

    #region CorridorWalk


    private IEnumerator RunCorridorWalkCoroutine()
    {
        var currentPosition = startPosition;
        HashSet<Vector3Int> positions = new();
        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleCorridorRandomWalk(currentPosition, walkLength, mapTiles.ToList());

            currentPosition = path.Last();
            positions.UnionWith(path);
            if (isRoomCreationIsPossible && Random.Range(0, 1) < probability)
            {
                RunRandomWalkCoroutine(currentPosition,false);
            }
            Debug.Log(i / iterations);
            LoadingPicture.fillAmount = (float)i / iterations;
            yield return null;
        }

        mapTiles.UnionWith(positions);
    }



    #endregion
    #region WallGeneration

    IEnumerator RunWallGeneration()
    {
        HashSet<Vector3Int> walls = new();

        for (int i = 0; i < mapTiles.Count; i++)
        {
            var currentTile = mapTiles.ElementAt(i);
            if (currentTile.z == (int)TileType.Floor)
            {
                Vector3Int[] adjacentPositions = new Vector3Int[]
                {
                    new(currentTile.x, currentTile.y + 1),//arrete commune
                    new (currentTile.x + 1, currentTile.y),//arrete commune
                    new (currentTile.x - 1, currentTile.y),//arrete commune
                    new (currentTile.x, currentTile.y - 1),//arrete commune
               
                    new (currentTile.x+1, currentTile.y - 1),//vertice commune
                    new (currentTile.x-1, currentTile.y - 1),//vertice commune
                    new (currentTile.x+1, currentTile.y + 1),//vertice commune
                    new (currentTile.x-1, currentTile.y + 1)//vertice commune
               };


                foreach (var pos in adjacentPositions)
                {
                    if (!mapTiles.Contains(pos) && !walls.Contains(pos))
                    {
                        walls.Add(new Vector3Int { x = pos.x, y = pos.y, z = -1 });
                    }
                }


            }
            LoadingPicture.fillAmount = (float)i / mapTiles.Count;

            yield return null;
        }
        UpdateWallsAfterAlgo(walls);

    }

    public void UpdateWallsAfterAlgo(HashSet<Vector3Int> walls)
    {

        HashSet<Vector2Int> wallPositions2Int = new(walls.Select(w => new Vector2Int(w.x, w.y)));
        for (int i = 0; i < walls.Count; i++)
        {
            Vector3Int pos = walls.ElementAt(i);

            var neibourghPresence = HasNeibourghIn(pos, wallPositions2Int);


            walls.Remove(pos);
            pos.z = (int)GetNewTileTypeWithNeibourghs(neibourghPresence);
            walls.Add(new Vector3Int { x = pos.x, y = pos.y, z = pos.z });
        }

        mapTiles.UnionWith(walls);
        tileMapVisualizer.PaintTiles();
    }

    public List<bool> HasNeibourghIn(Vector3Int myPos, HashSet<Vector2Int> positionsList)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
                   {
                new(myPos.x, myPos.y + 1), // voisin du haut
                new(myPos.x - 1, myPos.y), // voisin de gauche
                new(myPos.x + 1, myPos.y), // voisin de droite
                new(myPos.x, myPos.y - 1)  // voisin du bas
                   };


        return new(){
            positionsList.Contains(adjacentPositions[0]),
            positionsList.Contains(adjacentPositions[1]),
            positionsList.Contains(adjacentPositions[2]),
            positionsList.Contains(adjacentPositions[3])};
    }

    public TileType GetNewTileTypeWithNeibourghs(List<bool> neibourghPresence)
    {
        bool hasTop = neibourghPresence[0];
        bool hasLeft = neibourghPresence[1];
        bool hasRight = neibourghPresence[2];
        bool hasBottom = neibourghPresence[3];
        if (hasTop && hasRight && hasLeft && hasBottom)
        {
            return TileType.ConnexionAllDirectionsWall;
        }
        else if (hasTop && hasRight && hasLeft)
        {
            return TileType.ConnexionLeftTopRightWall;
        }
        else if (hasRight && hasBottom && hasLeft)
        {
            return TileType.ConnexionRightDownLeftWall;
        }
        else if (hasTop && hasLeft && hasBottom)
        {
            return TileType.ConnexionTopLeftDownWall;
        }
        else if (hasTop && hasRight && hasBottom)
        {
            return TileType.ConnexionTopRightDownWall;
        }
        else if (hasTop && hasRight)
        {
            return TileType.CornerDownLeftWall;
        }
        else if (hasTop && hasLeft)
        {
            return TileType.CornerDownRightWall;
        }
        else if (hasBottom && hasLeft)
        {
            return TileType.CornerTopRightWall;
        }
        else if (hasBottom && hasRight)
        {
            return TileType.CornerTopLeftWall;
        }
        else if (hasTop && hasBottom)
        {
            return TileType.StraightVerticalWall;
        }
        else if (hasLeft && hasRight)
        {
            return TileType.StraightHorizontalWall;
        }
        return TileType.Floor;
    }


    private void ClearWalls()
    {
        mapTiles.RemoveWhere((t) => t.z != (int)TileType.Floor);
    }
    #endregion

    #region Setters/Getters
    public void SetIsEachIterationStartFromRandomlyPosition(bool value)
    {
        isEachIterationStartFromRandomlyPosition = value;
    }

    public void SetIterations(string value)
    {
        int.TryParse(value, out iterations);
    }

    public void SetWalkLength(string value)
    {
        int.TryParse(value, out walkLength);
    }
    public void SetStartPosition(Vector3Int startPos)
    {
        startPosition = startPos;
    }
    public Vector3Int GetStartPosition()
    {
        return startPosition;
    }

    public HashSet<Vector3Int> GetMapTiles()
    {
        return mapTiles;
    }

    public HashSet<Vector3Int> SetMapTiles(List<Vector3Int> tileMap)
    {
        mapTiles.Clear();
        for (int i = 0; i < tileMap.Count; i++)
        {
            mapTiles.Add(new Vector3Int
            {
                x = tileMap[i].x,
                y = tileMap[i].y,
                z = tileMap[i].z,
            });
        }
        return mapTiles;
    }

    public void DeleteTile(Vector3Int tilePos)
    {
        for (int i = 0; i < mapTiles.Count; i++)
        {
            if (mapTiles.ElementAt(i).x == tilePos.x && mapTiles.ElementAt(i).y == tilePos.y)
            {
                mapTiles.Remove(mapTiles.ElementAt(i));
            }
        }
    }

    public void SetTileType(Vector3Int tilePos, int tiletype)
    {

        for (int i = 0; i < mapTiles.Count; i++)
        {
            var tile = mapTiles.ElementAt(i);
            if (tile.x == tilePos.x && tile.y == tilePos.y)
            {
                tile.z = tiletype;
                mapTiles.Add(tile);
                break;
            }
        }
        tileMapVisualizer.PaintTiles();
    }

    public void SetProbalityOfCreateRoom(string v)
    {
        float.TryParse(v, out probability);
    }

    public void SetIsRoomCreationIsPossible(bool value)
    {
        isRoomCreationIsPossible = value;
    }

}

public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2Int)
        {
            Vector2Int other = (Vector2Int)obj;
            return x == other.x && y == other.y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }
    #endregion
}
