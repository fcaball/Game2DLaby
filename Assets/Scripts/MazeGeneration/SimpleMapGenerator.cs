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
    private int _iterations = 0;
    private int _walkLength = 0;
    private float _probability = 0.0f;
    private bool _isRoomCreationIsPossible = true;
    private bool _isRoomGenerationIsON = false;
    private bool _isEachIterationStartFromRandomlyPosition = true;
    [SerializeField] private TileMapVisualizer _tileMapVisualizer;
    private HashSet<Vector3Int> _mapTiles = new();
    [SerializeField] private UnityEvent _missingParameter;
    [SerializeField] private GameObject _runButton;
    [SerializeField] private Image _loadingPicture;
    private int _currentAlgo;
    private List<Vector3Int> _potentialsRooms=new();
    [SerializeField]private UnityEvent<string> _refreshNumberOfPotentialRooms;

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
                    if (_isRoomCreationIsPossible && _potentialsRooms.Count>0)
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

            case 1:
                ClearWalls();
                StartCoroutine(RunProceduralGenerationCoroutine(RunWallGeneration()));
                break;
            case 2:
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
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, _walkLength, _mapTiles.ToList());

            positions.UnionWith(path);
            if (_isEachIterationStartFromRandomlyPosition)
            {
                int index = Random.Range(0, positions.Count);
                while (positions.ElementAt(index).z == (int)TileType.StraightVerticalWall)
                {
                    index = Random.Range(0, positions.Count);
                }
                currentPosition = new Vector3Int(positions.ElementAt(index).x, positions.ElementAt(index).y, 0);
            }
            if (isLoadingHaveToBeUpdated)
                _loadingPicture.fillAmount = (float)i / _iterations;
            yield return null;
        }
        _mapTiles.UnionWith(positions);
    }
    #endregion

    #region CorridorWalk


    private IEnumerator RunCorridorWalkCoroutine()
    {
        var currentPosition = startPosition;
        HashSet<Vector3Int> positions = new();
        for (int i = 0; i < _iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleCorridorRandomWalk(currentPosition, _walkLength, _mapTiles.ToList());

            currentPosition = path.Last();
            positions.UnionWith(path);
            if (_isRoomCreationIsPossible && Random.Range(0.0f, 1.0f) < (_probability / 100.0f))
            {
                _potentialsRooms.Add(currentPosition);
                _refreshNumberOfPotentialRooms.Invoke(_potentialsRooms.Count+ " pièce(s) potentielle(s)");
            }
            Debug.Log(i / _iterations);
            _loadingPicture.fillAmount = (float)i / _iterations;
            yield return null;
        }

        _mapTiles.UnionWith(positions);
    }



    #endregion
    #region WallGeneration

    IEnumerator RunWallGeneration()
    {
        HashSet<Vector3Int> walls = new();

        for (int i = 0; i < _mapTiles.Count; i++)
        {
            var currentTile = _mapTiles.ElementAt(i);
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
                    if (!_mapTiles.Contains(pos) && !walls.Contains(pos))
                    {
                        walls.Add(new Vector3Int { x = pos.x, y = pos.y, z = -1 });
                    }
                }


            }
            _loadingPicture.fillAmount = (float)i / _mapTiles.Count;

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

        _mapTiles.UnionWith(walls);
        _tileMapVisualizer.PaintTiles();
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
        _mapTiles.RemoveWhere((t) => t.z != (int)TileType.Floor);
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

    public HashSet<Vector3Int> GetMapTiles()
    {
        return _mapTiles;
    }

    public HashSet<Vector3Int> SetMapTiles(List<Vector3Int> tileMap)
    {
        _mapTiles.Clear();
        for (int i = 0; i < tileMap.Count; i++)
        {
            _mapTiles.Add(new Vector3Int
            {
                x = tileMap[i].x,
                y = tileMap[i].y,
                z = tileMap[i].z,
            });
        }
        return _mapTiles;
    }

    public void DeleteTile(Vector3Int tilePos)
    {
        for (int i = 0; i < _mapTiles.Count; i++)
        {
            if (_mapTiles.ElementAt(i).x == tilePos.x && _mapTiles.ElementAt(i).y == tilePos.y)
            {
                _mapTiles.Remove(_mapTiles.ElementAt(i));
            }
        }
    }

    public void SetTileType(Vector3Int tilePos, int tiletype)
    {

        for (int i = 0; i < _mapTiles.Count; i++)
        {
            var tile = _mapTiles.ElementAt(i);
            if (tile.x == tilePos.x && tile.y == tilePos.y)
            {
                tile.z = tiletype;
                _mapTiles.Add(tile);
                break;
            }
        }
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

}
#endregion


