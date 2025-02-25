using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{

    [SerializeField] private Tilemap _map;
    [SerializeField] private List<TileBase> _codeTiles;
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
        HashSet<Vector3Int> TileDatas = _simpleMapGenerator.GetMapTiles();
        foreach (var Vector3Int in TileDatas)
        {
            PaintSingleTile(tilemap, _codeTiles[Vector3Int.z], new Vector3Int(Vector3Int.x, Vector3Int.y));
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector3Int position)
    {
        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), tile);
        _tilemapChanged.Invoke();
    }

    public void Clear(bool isTotalReset = false)
    {
        HashSet<Vector3Int> floorTiles = _simpleMapGenerator.GetMapTiles();

        _map.ClearAllTiles();
        floorTiles.Clear();
        if (!isTotalReset)
        {
            Vector3Int startPosition = _simpleMapGenerator.GetStartPosition();
            floorTiles.Add(new Vector3Int() { x = startPosition.x, y = startPosition.y, z = (int)TileType.Floor });
            _map.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), _codeTiles[(int)TileType.Floor]);
        }
        else
        {
            _resetStartPosition.Invoke();
        }

    }

    public void AddTile(Vector3Int cellPosition, TileType tileType)
    {
        HashSet<Vector3Int> floorTiles = _simpleMapGenerator.GetMapTiles();

        floorTiles.Add(new Vector3Int() { x = cellPosition.x, y = cellPosition.y, z = (int)tileType });
        PaintSingleTile(_map, _codeTiles[(int)tileType], cellPosition);
    }

    public HashSet<Vector3Int> GetMapTiles()
    {
        HashSet<Vector3Int> floorTiles = _simpleMapGenerator.GetMapTiles();

        return floorTiles;
    }

    public Tilemap GetTileMap()
    {
        return _map;
    }

    public void LoadFloor(int index)
    {
        _map.ClearAllTiles();
        _simpleMapGenerator.SetMapTiles(MazeData.MazeFloors[index].TileMap);
        PaintTiles();
    }

    public void ChangeTileType(Vector3Int tilePos, int tileType)
    {
        // PaintSingleTile(Map, codeTiles[tileType], tilePos);
        _simpleMapGenerator.SetTileType(tilePos, tileType);
    }

    public bool SetInOut(Vector3Int tilePos)
    {
       if(_map.GetTile(tilePos)==_codeTiles[(int)TileType.Floor]){

        return true;
       }else{
        return false;
       }
    }
}
