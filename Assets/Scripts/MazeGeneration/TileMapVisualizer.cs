using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{

    [SerializeField] private Tilemap Map;
    [SerializeField] private List<TileBase> codeTiles;
    [SerializeField] private SimpleMapGenerator simpleMapGenerator;
    [SerializeField] private FloorsEditorManager floorsEditorManager;
    [SerializeField] private UnityEvent TilemapChanged;
    [SerializeField] private UnityEvent ResetStartPosition;

    private void Start()
    {

        if (MazeData.mazeFloors.Count > 0)
            LoadFloor(0);
    }

    public void PaintTiles()
    {
        Map.ClearAllTiles();
        PaintTiles(Map);
    }

    private void PaintTiles(Tilemap tilemap)
    {
        HashSet<Vector3Int> TileDatas = simpleMapGenerator.GetMapTiles();
        foreach (var Vector3Int in TileDatas)
        {
            PaintSingleTile(tilemap, codeTiles[Vector3Int.z], new Vector3Int(Vector3Int.x, Vector3Int.y));
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector3Int position)
    {
        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), tile);
        TilemapChanged.Invoke();
    }

    public void Clear(bool isTotalReset = false)
    {
        HashSet<Vector3Int> floorTiles = simpleMapGenerator.GetMapTiles();

        Map.ClearAllTiles();
        floorTiles.Clear();
        if (!isTotalReset)
        {
            Vector3Int startPosition = simpleMapGenerator.GetStartPosition();
            floorTiles.Add(new Vector3Int() { x = startPosition.x, y = startPosition.y, z = (int)TileType.Floor });
            Map.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), codeTiles[(int)TileType.Floor]);
        }
        else
        {
            ResetStartPosition.Invoke();
        }

    }

    public void AddTile(Vector3Int cellPosition, TileType tileType)
    {
        HashSet<Vector3Int> floorTiles = simpleMapGenerator.GetMapTiles();

        floorTiles.Add(new Vector3Int() { x = cellPosition.x, y = cellPosition.y, z = (int)tileType });
        PaintSingleTile(Map, codeTiles[(int)tileType], cellPosition);
    }

    public HashSet<Vector3Int> GetMapTiles()
    {
        HashSet<Vector3Int> floorTiles = simpleMapGenerator.GetMapTiles();

        return floorTiles;
    }

    public Tilemap GetTileMap()
    {
        return Map;
    }

    public void LoadFloor(int index)
    {
        Map.ClearAllTiles();
        simpleMapGenerator.SetMapTiles(MazeData.mazeFloors[index].tileMap);
        PaintTiles();
    }

    public void ChangeTileType(Vector3Int tilePos, int tileType)
    {
        // PaintSingleTile(Map, codeTiles[tileType], tilePos);
        simpleMapGenerator.SetTileType(tilePos, tileType);
    }
}
