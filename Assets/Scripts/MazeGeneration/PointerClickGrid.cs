using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.EventSystems;

public class PointerClickGrid : MonoBehaviour
{
    private Tilemap _tilemap; // Référence à la Tilemap
    [SerializeField] private Tilemap _tilemapUIClicked; 
    [SerializeField] private Tilemap _tilemapUIHovered; 
    [SerializeField] private Tilemap _tilemapStartPosition; 
    [SerializeField] private Tilemap _tilemapInOutAndMinerals; 
    [SerializeField] private TileMapVisualizer _tileMapVisualizer;
    [SerializeField] private SimpleMapGenerator _simpleMapGenerator;
    private CameraControl _cameraControl;
    [SerializeField] private Button _startPosition;

    [SerializeField] private TileBase _selectTileBase;
    [SerializeField] private TileBase _startTileBase;
    [SerializeField] private TileBase _hoverTileBase;
    [SerializeField] private TMPro.TMP_InputField _brushSizeInput;
    private List<Vector3Int> _previousClickedTiles = new();
    private List<Vector3Int> _previousHoveredTile = new();
    [SerializeField] private int _brushSize = 1;
    [SerializeField] private UnityEvent _blockRun;
    [SerializeField] private UnityEvent _unBlockRun;
    [SerializeField] private Image _imageBlocker;

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _tilemap.GetComponent<TilemapRenderer>().sortingOrder = 0;
        _tilemapUIClicked.GetComponent<TilemapRenderer>().sortingOrder = 1;
        _tilemapUIHovered.GetComponent<TilemapRenderer>().sortingOrder = 2;
        _tilemapStartPosition.GetComponent<TilemapRenderer>().sortingOrder = 3;
        _tilemapInOutAndMinerals.GetComponent<TilemapRenderer>().sortingOrder = 4;
        _cameraControl = Camera.main.gameObject.GetComponent<CameraControl>();
    }




    public void OnClickTile(List<Vector3Int> cellPositions)
    {
        _tilemapUIClicked.ClearAllTiles();

        foreach (var cellPosition in cellPositions)
        {
            _tilemapUIClicked.SetTile(cellPosition, _selectTileBase);
        }

        _startPosition.interactable = _previousClickedTiles.Count == 1;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (_brushSize + (int)(Input.GetAxis("Mouse ScrollWheel") * 10) >= 1)
            {
                _brushSize += (int)(Input.GetAxis("Mouse ScrollWheel") * 10);
                _brushSizeInput.text = _brushSize.ToString();
            }
        }
        if (!_imageBlocker.raycastTarget && !_cameraControl.IsDragging)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = _tilemap.transform.position.z;

            Vector3Int cellPosition = _tilemap.WorldToCell(worldPosition);
            _tilemapUIHovered.ClearAllTiles();
            _previousHoveredTile = GetPositionsWithBrushSize(cellPosition);
            foreach (var tilePos in _previousHoveredTile)
            {
                _tilemapUIHovered.SetTile(tilePos, _hoverTileBase);
            }

            if (Input.GetMouseButton(0) && !IsPointerOverUIElement())
            {
                bool IsDeleting = Input.GetKey(KeyCode.LeftShift);
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    _previousClickedTiles.Clear();
                }
                List<Vector3Int> tilesInBrush = GetPositionsWithBrushSize(cellPosition);
                foreach (var tilePos in tilesInBrush)
                {
                    if (IsDeleting)
                        _previousClickedTiles.Remove(tilePos);
                    else
                        _previousClickedTiles.Add(tilePos);
                }
                OnClickTile(_previousClickedTiles);
            }

            if (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace))
            {
                DeleteTiles();
            }


        }
        if (_cameraControl.IsDragging)
        {
            _tilemapUIClicked.ClearAllTiles();

        }

    }

    public static bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }

        return false;
    }

    public void DeleteTiles()
    {

        foreach (var tilePos in _previousClickedTiles)
        {
            if (_tilemapStartPosition.GetTile(tilePos) != _startTileBase)
            {
                var Inout=MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.Find(t => t.Location == tilePos && t.IndexFloorDestination==MazeData.CurrentFloor);
                if ( Inout!= null)
                {
                    _tilemapInOutAndMinerals.SetTile(Inout.SpawnInDestination, null);
                }
                _tilemap.SetTile(tilePos, null);
                _tilemapUIClicked.SetTile(tilePos, null);
                _simpleMapGenerator.DeleteTile(tilePos);
                _tilemapInOutAndMinerals.SetTile(tilePos, null);
            }
            _tilemapUIClicked.SetTile(tilePos, null);
        }
        _previousClickedTiles.Clear();
        _startPosition.interactable = _previousClickedTiles.Count == 1;

    }

    public void SetStartPosition()
    {
        _tilemapStartPosition.SetTile(_simpleMapGenerator.GetStartPosition(), null);
        _simpleMapGenerator.SetStartPosition(_previousClickedTiles[0]);
        _tilemapStartPosition.SetTile(_previousClickedTiles[0], _startTileBase);
        _unBlockRun.Invoke();
    }
    public void ResetStartPosition()
    {
        _tilemapStartPosition.SetTile(_simpleMapGenerator.GetStartPosition(), null);
        _blockRun.Invoke();
    }

    public void ChangeTileType(int tileType)
    {
        foreach (var tile in _previousClickedTiles)
        {
            _simpleMapGenerator.SetTileType(tile, tileType);
            _tilemapUIClicked.SetTile(tile, null);
        }
        _previousClickedTiles.Clear();
        _tilemapStartPosition.ClearAllTiles();
        _startPosition.interactable = _previousClickedTiles.Count == 1;
    }

    public void SetInOut(int floorDestination, Vector3Int positionDestinationSpawn)
    {
        foreach (var tilePos in _previousClickedTiles)
        {
            _tileMapVisualizer.SetInOut(tilePos, floorDestination, positionDestinationSpawn);

        }
    }

    public void SetBrushSize(string brushSize)
    {
        _brushSize = Convert.ToInt32(brushSize);
    }


    public List<Vector3Int> GetPositionsWithBrushSize(Vector3Int center)
    {
        List<Vector3Int> tilePositions = new List<Vector3Int>();
        int rayon = _brushSize / 2;

        for (int x = -rayon; x <= rayon; x++)
        {
            for (int y = -rayon; y <= rayon; y++)
            {
                Vector3Int tilePos = new Vector3Int(center.x + x, center.y + y, center.z);

                if (_brushSize % 2 == 0 && x * x + y * y <= rayon * rayon)
                {
                    tilePositions.Add(tilePos);
                }
                else if (_brushSize % 2 == 1)
                {
                    tilePositions.Add(tilePos);
                }
            }
        }

        return tilePositions;
    }

    public void AddMinerals(){
         foreach (var tilePos in _previousClickedTiles)
        {
            _tileMapVisualizer.SetMineral(tilePos);
        }
    }
}
