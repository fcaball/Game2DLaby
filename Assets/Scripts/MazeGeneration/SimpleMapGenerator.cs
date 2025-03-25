using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;

public class SimpleMapGenerator : MonoBehaviour
{
    [SerializeField] protected Vector3Int startPosition;
    private int _iterations = 0;
    private int _walkLength = 0;
    private float _probability = 0.0f;
    private bool _isRoomCreationIsPossible = true;
    private bool _isRoomGenerationIsON = false;
    private bool _isEachIterationStartFromRandomlyPosition = true;
    [SerializeField] private TileMapVisualizer _tileMapVisualizer;
    [SerializeField] private UnityEvent _missingParameter;
    [SerializeField] private GameObject _runButton;
    [SerializeField] private Image _loadingPicture;
    private int _currentAlgo;
    private List<Vector3Int> _potentialsRooms = new();
    [SerializeField] private UnityEvent<string> _refreshNumberOfPotentialRooms;

    public void SetCurrentAlgo(int index)
    {
        _currentAlgo = index;
    }

    #region General Run
    public void Run()
    {

        switch (_currentAlgo)
        {
            case 0:
                if (_iterations == 0 || _walkLength == 0)
                {
                    _missingParameter.Invoke();
                }
                else
                {
                    if (_isRoomCreationIsPossible && _potentialsRooms.Count > 0)
                    {
                        foreach (var RoomStart in _potentialsRooms)
                        {
                            StartCoroutine(RunProceduralGenerationCoroutine(RunRandomWalkCoroutine(RoomStart, true)));
                        }
                    }
                    else
                    {
                        StartCoroutine(RunProceduralGenerationCoroutine(RunRandomWalkCoroutine(startPosition, true)));
                    }
                    SetIsRoomGenerationIsON(false);

                }
                break;

            case 2:
                ClearWalls();
                StartCoroutine(RunProceduralGenerationCoroutine(RunWallGeneration()));
                break;
            case 1:
                if (_iterations == 0 || _walkLength == 0)
                {
                    _missingParameter.Invoke();
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
        _runButton.SetActive(false);
        _loadingPicture.gameObject.SetActive(true);

        yield return StartCoroutine(enumerator);

        _tileMapVisualizer.PaintTiles();
        _loadingPicture.fillAmount = 0;
        _loadingPicture.gameObject.SetActive(false);
        _runButton.SetActive(true);
    }

    #endregion

    #region RandomWalk


    private IEnumerator RunRandomWalkCoroutine(Vector3Int startPos, bool isLoadingHaveToBeUpdated)
    {
        var currentPosition = startPos;
        HashSet<Vector3Int> positions = new();
        for (int i = 0; i < _iterations; i++)
        {
            Debug.Log("Iter");
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, _walkLength, MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos);

            positions.UnionWith(path);
            if (_isEachIterationStartFromRandomlyPosition)
            {
                int index = Random.Range(0, positions.Count);
                currentPosition = new Vector3Int(positions.ElementAt(index).x, positions.ElementAt(index).y, 0);
            }
            if (isLoadingHaveToBeUpdated)
                _loadingPicture.fillAmount = (float)i / _iterations;
            yield return null;
        }
        MazeData.MazeFloors[MazeData.CurrentFloor].AddOrReplaceTiles(positions);
        // _mapTiles.UnionWith(positions);
    }
    #endregion

    #region CorridorWalk


    private IEnumerator RunCorridorWalkCoroutine()
    {
        var currentPosition = startPosition;
        HashSet<Vector3Int> positions = new();
        for (int i = 0; i < _iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleCorridorRandomWalk(currentPosition, _walkLength, MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos);

            currentPosition = path.Last();
            positions.UnionWith(path);
            if (_isRoomCreationIsPossible && Random.Range(0.0f, 1.0f) < (_probability / 100.0f))
            {
                _potentialsRooms.Add(currentPosition);
                _refreshNumberOfPotentialRooms.Invoke(_potentialsRooms.Count + " pièce(s) potentielle(s)");
            }
            Debug.Log(i / _iterations);
            _loadingPicture.fillAmount = (float)i / _iterations;
            yield return null;
        }

        MazeData.MazeFloors[MazeData.CurrentFloor].AddOrReplaceTiles(positions);

    }

    public void ClearRooms()
    {
        _potentialsRooms.Clear();
        _refreshNumberOfPotentialRooms.Invoke(_potentialsRooms.Count + " pièce(s) potentielle(s)");

    }



    #endregion
    #region WallGeneration

    IEnumerator RunWallGeneration()
    {
        HashSet<Vector3Int> walls = new();
        var floorDatas = MazeData.MazeFloors[MazeData.CurrentFloor];

        for (int i = 0; i < floorDatas.TileInfos.Count; i++)
        {
            var currentTile = floorDatas.TileInfos[i];
            if (currentTile.Tag == (int)TileTag.Floor)
            {
                Vector3Int[] adjacentPositions = new Vector3Int[]
                {
                    new(currentTile.Position.x, currentTile.Position.y + 1),//arrete commune
                    new (currentTile.Position.x + 1, currentTile.Position.y),//arrete commune
                    new (currentTile.Position.x - 1, currentTile.Position.y),//arrete commune
                    new (currentTile.Position.x, currentTile.Position.y - 1),//arrete commune
               
                    new (currentTile.Position.x+1, currentTile.Position.y - 1),//vertice commune
                    new (currentTile.Position.x-1, currentTile.Position.y - 1),//vertice commune
                    new (currentTile.Position.x+1, currentTile.Position.y + 1),//vertice commune
                    new (currentTile.Position.x-1, currentTile.Position.y + 1)//vertice commune
               };


                foreach (var pos in adjacentPositions)
                {
                    if (!floorDatas.ContainsTile(pos) && !walls.Contains(pos))
                    {
                        walls.Add(new Vector3Int { x = pos.x, y = pos.y, z = 0 });
                    }
                }


            }
            _loadingPicture.fillAmount = (float)i / floorDatas.TileInfos.Count;

            yield return null;
        }
        UpdateWallsAfterAlgo(walls);

    }

    public HashSet<Vector3Int> UpdateWallsAfterAlgo(HashSet<Vector3Int> walls)
    {
        List<TileDatas> tileDatas = new();
        HashSet<Vector2Int> wallPositions2Int = new(walls.Select(w => new Vector2Int(w.x, w.y)));
        for (int i = 0; i < walls.Count; i++)
        {
            Vector3Int pos = walls.ElementAt(i);

            var neibourghPresence = HasNeibourghIn(pos, wallPositions2Int);

            TileTag TileType = GetNewTileTypeWithNeibourghs(neibourghPresence);
            tileDatas.Add(new()
            {
                Position = pos,
                Tag = TileType,
                Rotation = TileDatas.GetTileRotation(TileType)
            });
        }

        MazeData.MazeFloors[MazeData.CurrentFloor].AddOrReplaceTiles(tileDatas);

        _tileMapVisualizer.PaintTiles();
        return walls;
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

    public TileTag GetNewTileTypeWithNeibourghs(List<bool> neibourghPresence)
    {
        bool hasTop = neibourghPresence[0];
        bool hasLeft = neibourghPresence[1];
        bool hasRight = neibourghPresence[2];
        bool hasBottom = neibourghPresence[3];
        if (hasTop && hasRight && hasLeft && hasBottom)
        {
            return TileTag.ConnexionAllDirectionsWall;
        }
        else if (hasTop && hasRight && hasLeft)
        {
            return TileTag.ConnexionLeftTopRightWall;
        }
        else if (hasRight && hasBottom && hasLeft)
        {
            return TileTag.ConnexionRightDownLeftWall;
        }
        else if (hasTop && hasLeft && hasBottom)
        {
            return TileTag.ConnexionTopLeftDownWall;
        }
        else if (hasTop && hasRight && hasBottom)
        {
            return TileTag.ConnexionTopRightDownWall;
        }
        else if (hasTop && hasRight)
        {
            return TileTag.CornerDownLeftWall;
        }
        else if (hasTop && hasLeft)
        {
            return TileTag.CornerDownRightWall;
        }
        else if (hasBottom && hasLeft)
        {
            return TileTag.CornerTopRightWall;
        }
        else if (hasBottom && hasRight)
        {
            return TileTag.CornerTopLeftWall;
        }
        else if (hasTop && hasBottom)
        {
            return TileTag.StraightVerticalWall;
        }
        else if (hasLeft && hasRight)
        {
            return TileTag.StraightHorizontalWall;
        }
        Debug.Log("floor choosed");
        return TileTag.Floor;
    }


    private void ClearWalls()
    {
        MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos.RemoveAll((t) => t.Tag != (int)TileTag.Floor);
    }
    #endregion

    #region Setters/Getters
    public void SetIsEachIterationStartFromRandomlyPosition(bool value)
    {
        _isEachIterationStartFromRandomlyPosition = value;
    }

    public void SetIsRoomGenerationIsON(bool value)
    {
        _isRoomGenerationIsON = value;
    }


    public void SetIterations(string value)
    {
        int.TryParse(value, out _iterations);
    }

    public void SetWalkLength(string value)
    {
        int.TryParse(value, out _walkLength);
    }
    public void SetStartPosition(Vector3Int startPos)
    {
        startPosition = startPos;
    }
    public Vector3Int GetStartPosition()
    {
        return startPosition;
    }


    public void DeleteTile(Vector3Int tilePos)
    {
        MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos.RemoveAll((t) => t.Position == tilePos);
        MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.RemoveAll((io) => io.Location == tilePos);
        MazeData.MazeFloors[MazeData.CurrentFloor].Minerals.Remove(tilePos);
    }

    public void SetTileType(Vector3Int tilePos, int tiletype)
    {
        var _mapTiles = MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos;
        TileDatas tileDatas = _mapTiles
    .FirstOrDefault(tD => tD.Position.x == tilePos.x && tD.Position.y == tilePos.y);

        if (tileDatas == null)
        {
            _mapTiles.Add(new TileDatas()  // Ajoute un nouvel objet correctement instancié
            {
                Position = tilePos,
                Tag = (TileTag)tiletype,
                Rotation = TileDatas.GetTileRotation((TileTag)tiletype)
            });
        }
        else
        {
            tileDatas.Tag = (TileTag)tiletype;
            tileDatas.Rotation = TileDatas.GetTileRotation((TileTag)tiletype);
        }


        // for (int i = 0; i < _mapTiles.Count; i++)
        // {
        //     var tile = _mapTiles.ElementAt(i);
        //     if (tile.Position.x == tilePos.x && tile.Position.y == tilePos.y)
        //     {
        //         _mapTiles.Remove(_mapTiles.ElementAt(i));
        //         tile.Tag = (TileTag)tiletype;
        //         tile.Rotation = TileDatas.GetTileRotation((TileTag)tiletype);
        //         _mapTiles.Add(tile);
        //         break;
        //     }
        // }
        _tileMapVisualizer.PaintTiles();
    }

    public void SetProbalityOfCreateRoom(string v)
    {
        float.TryParse(v, out _probability);
    }

    public void SetIsRoomCreationIsPossible(bool value)
    {
        _isRoomCreationIsPossible = value;
    }


    #endregion


    public void AddInOut(Vector3Int pos, int floorDestination, Vector3Int positionDestinationSpawn)
    {
        MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.Add(new InOut(pos, floorDestination, positionDestinationSpawn));
    }

    public void RemoveInOut(Vector3Int pos)
    {
        MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.RemoveAll((io) => io.Location == pos);
    }
}