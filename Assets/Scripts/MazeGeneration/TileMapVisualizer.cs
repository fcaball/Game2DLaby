using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{

    [SerializeField] private Tilemap _map;
    [SerializeField] private Tilemap _tilemapInOutAndMinerals;
    [SerializeField] private List<TileBase> _codeTiles;
    [SerializeField] private TileBase _inOutTileBase;
    [SerializeField] private TileBase _destinationTileBase;
    [SerializeField] private TileBase _mineralTileBase;
    [SerializeField] private SimpleMapGenerator _simpleMapGenerator;
    [SerializeField] private FloorsEditorManager _floorsEditorManager;
    [SerializeField] private UnityEvent _tilemapChanged;
    [SerializeField] private UnityEvent _resetStartPosition;

    private void Start()
    {

        if (MazeData.MazeFloors.Count > 0)
            LoadFloor(0);
    }

    public void PaintTiles()
    {
        _map.ClearAllTiles();
        PaintTiles(_map);
    }

    private void PaintTiles(Tilemap tilemap)
    {
        List<TileDatas> TilesDatas = MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos;
        foreach (var tileDatas in TilesDatas)
        {
            PaintSingleTile(tilemap, tileDatas);
        }
    }

    private TileBase GetTileFromType(TileTag tag)
    {
        if (tag == TileTag.StraightVerticalWall || tag == TileTag.StraightHorizontalWall)
        {
            var tile = _codeTiles[1];

            return tile;
        }
        else if (tag == TileTag.CornerDownLeftWall || tag == TileTag.CornerTopLeftWall || tag == TileTag.CornerDownRightWall || tag == TileTag.CornerTopRightWall)
        {
            var tile = _codeTiles[2];

            return tile;
        }
        else if (tag == TileTag.ConnexionLeftTopRightWall || tag == TileTag.ConnexionRightDownLeftWall || tag == TileTag.ConnexionTopLeftDownWall || tag == TileTag.ConnexionTopRightDownWall)
        {
            var tile = _codeTiles[3];

            return tile;
        }
        else if (tag == TileTag.ConnexionAllDirectionsWall)
        {
            var tile = _codeTiles[4];

            return tile;
        }
        else
        {
            return _codeTiles[0];
        }

    }


    private void PaintSingleTile(Tilemap tilemap, TileDatas tileDatas)
    {
        tilemap.SetTile(tileDatas.Position, GetTileFromType(tileDatas.Tag));

        tilemap.SetTileFlags(tileDatas.Position, TileFlags.None); // Permet de modifier la transformation
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(tileDatas.Rotation)); // Rotation de 90°
        tilemap.SetTransformMatrix(tileDatas.Position, rotationMatrix);

        _tilemapChanged.Invoke();
    }

    public void Clear(bool isTotalReset = false)
    {
        MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos.Clear();
        MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.Clear();

        _map.ClearAllTiles();
        _tilemapInOutAndMinerals.ClearAllTiles();
        if (!isTotalReset)
        {
            Vector3Int startPosition = _simpleMapGenerator.GetStartPosition();
            MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos.Add(new()
            {
                Position = startPosition,
                Rotation = Vector3Int.zero,
                Tag = TileTag.Floor,
            });
            _map.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), _codeTiles[0]);
        }
        else
        {
            _resetStartPosition.Invoke();
        }

    }

    public void AddTile(Vector3Int cellPosition, TileTag tileType)
    {
        TileDatas tileDatas = new()
        {
            Position = cellPosition,
            Rotation = TileDatas.GetTileRotation(tileType),
            Tag = tileType
        };
        MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos.Add(tileDatas);

        PaintSingleTile(_map, tileDatas);
    }



    public Tilemap GetTileMap()
    {
        return _map;
    }

    public void LoadFloor(int index)
    {
        MazeData.CurrentFloor = index;
        _map.ClearAllTiles();

        _tilemapInOutAndMinerals.ClearAllTiles();
        initInOut();

        PaintTiles();
    }


    public void ChangeTileType(Vector3Int tilePos, int tileType)
    {
        _simpleMapGenerator.SetTileType(tilePos, tileType);
    }

    private void initInOut()
    {
        var InOuts = MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties;
        foreach (var InOut in InOuts)
        {
            _tilemapInOutAndMinerals.SetTile(InOut.Location, _inOutTileBase);
        }
        List<InOut> Destinations = MazeData.MazeFloors
     .SelectMany(e => e.EntreeSorties.Where(io => io.IndexFloorDestination == MazeData.CurrentFloor))
     .ToList();

        foreach (var arrival in Destinations)
        {
            Debug.Log(arrival.SpawnInDestination);
            _tilemapInOutAndMinerals.SetTile(arrival.SpawnInDestination, _destinationTileBase);
        }

    }

    public void SetInOut(Vector3Int tilePos, int floorDestination, Vector3Int positionDestinationSpawn)
    {
        if (MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.Find((io) => io.Location == tilePos) != null)
        {
            _simpleMapGenerator.RemoveInOut(tilePos);
            _tilemapInOutAndMinerals.SetTile(tilePos, null);
            if (floorDestination == MazeData.CurrentFloor)
            {
                _tilemapInOutAndMinerals.SetTile(positionDestinationSpawn, null);
            }
        }
        else
        {
            _simpleMapGenerator.AddInOut(tilePos, floorDestination, positionDestinationSpawn);
            _tilemapInOutAndMinerals.SetTile(tilePos, _inOutTileBase);
            if (floorDestination == MazeData.CurrentFloor)
            {
                _tilemapInOutAndMinerals.SetTile(positionDestinationSpawn, _destinationTileBase);
            }
        }
    }

    public void SetMineral(Vector3Int tilePos)
    {
        var currentFloor=MazeData.MazeFloors[MazeData.CurrentFloor];
        if (currentFloor.TileInfos.Find(t=> t.Position==tilePos && t.Tag==TileTag.Floor) != null && _tilemapInOutAndMinerals.GetTile(tilePos)==null){
                currentFloor.Minerals.Add(tilePos);
                _tilemapInOutAndMinerals.SetTile(tilePos,_mineralTileBase);
        }        
    }
}
