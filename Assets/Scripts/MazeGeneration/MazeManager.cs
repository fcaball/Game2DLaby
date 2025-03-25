using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeManager : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemapBackground;
    [SerializeField] private Tilemap _tilemapForWrongsEmplacement;
    [SerializeField] private List<TileBase> _codeTiles;
    [SerializeField] private TileBase _redCrossTileBase;

    private void Start()
    {
        _tilemapBackground.GetComponent<TilemapRenderer>().sortingOrder = 0;
        _tilemapForWrongsEmplacement.GetComponent<TilemapRenderer>().sortingOrder = 1;
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

    public void SimpleLoadFloor(int floorIndex)
    {
        _tilemapBackground.ClearAllTiles();
        _tilemapForWrongsEmplacement.ClearAllTiles();
        foreach (var tile in MazeData.MazeFloors[floorIndex].TileInfos)
        {
            _tilemapBackground.SetTile(tile.Position, GetTileFromType(tile.Tag));
            _tilemapBackground.SetTileFlags(tile.Position, TileFlags.None); // Permet de modifier la transformation
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(tile.Rotation)); // Rotation de 90°
            _tilemapBackground.SetTransformMatrix(tile.Position, rotationMatrix);
            List<InOut> Destinations = MazeData.MazeFloors
              .SelectMany(e => e.EntreeSorties.Where(io => io.IndexFloorDestination == floorIndex))
              .ToList();
            if (MazeData.MazeFloors[floorIndex].Minerals.Contains(tile.Position) || MazeData.MazeFloors[floorIndex].EntreeSorties.Find(io => io.Location == tile.Position) != null || Destinations.Find(d=>d.SpawnInDestination==tile.Position)!=null)
            {
                _tilemapForWrongsEmplacement.SetTile(tile.Position, _redCrossTileBase);
            }
        }
    }

    public Vector3 GetWorldStartPosition()
    {
        Vector3Int startPos = _tilemapBackground.cellBounds.min;
        return _tilemapBackground.CellToWorld(startPos);
    }

    public void OnInOutEnter(Vector2Int tilePosition)
    {
        Debug.Log("Tu veux partir ?? " + tilePosition);
    }
}
