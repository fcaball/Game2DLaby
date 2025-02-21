using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class PointerClickGrid : MonoBehaviour, IPointerClickHandler/* , IPointerEnterHandler */
{
    private Tilemap _tilemap; // Référence à la Tilemap
    [SerializeField] private Tilemap _tilemapUI; // Référence à la Tilemap
    [SerializeField] private Tilemap _tilemapUI2; // Référence à la Tilemap
    [SerializeField] private TileMapVisualizer _tileMapVisualizer;
    [SerializeField] private SimpleMapGenerator _simpleMapGenerator;
    [SerializeField] private CameraControl _cameraControl;
    [SerializeField] private Button _startPosition;

    [SerializeField] private TileBase _selectTileBase;
    [SerializeField] private TileBase _startTileBase;
    [SerializeField] private TileBase _hoverTileBase;
    private List<Vector3Int> _previousClickedTiles = new();
    private Vector3Int _previousHoveredTile;
    [SerializeField] private UnityEvent _blockRun;
    [SerializeField] private UnityEvent _unBlockRun;
    [SerializeField] private Image _imageBlocker;

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _tilemap.GetComponent<TilemapRenderer>().sortingOrder = 0;
        _tilemapUI.GetComponent<TilemapRenderer>().sortingOrder = 1;
        _tilemapUI2.GetComponent<TilemapRenderer>().sortingOrder = 2;
        _cameraControl = Camera.main.gameObject.GetComponent<CameraControl>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // Convertir la position du clic de la souris en coordonnées du monde
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = _tilemap.transform.position.z; // Ajuster la profondeur Z

        // Convertir les coordonnées du monde en coordonnées de cellule
        Vector3Int cellPosition = _tilemap.WorldToCell(worldPosition);
        OnClickTile(cellPosition);

    }

    public void OnClickTile(Vector3Int cellPosition)
    {
        if (_tilemapUI.GetTile(cellPosition) == _selectTileBase &&
         (Input.GetKey(KeyCode.LeftControl) || (!Input.GetKey(KeyCode.LeftControl) && _previousClickedTiles.Count == 1)))
        {
            _tilemapUI.SetTile(cellPosition, null);
            _previousClickedTiles.Remove(cellPosition);
        }
        else
        {

            if (!Input.GetKey(KeyCode.LeftControl))
            {
                foreach (var position in _previousClickedTiles)
                {
                    _tilemapUI.SetTile(position, null);
                }
                _previousClickedTiles.Clear();
            }
            _tilemapUI.SetTile(cellPosition, _selectTileBase);
            _previousClickedTiles.Add(cellPosition);
        }
        _startPosition.interactable = _previousClickedTiles.Count == 1;
    }

    private void Update()
    {
        if (!_imageBlocker.raycastTarget && !_cameraControl.IsDragging)
        { // //  Convertir la position du clic de la souris en coordonnées du monde
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = _tilemap.transform.position.z; // Ajuster la profondeur Z

            // // Convertir les coordonnées du monde en coordonnées de cellule
            Vector3Int cellPosition = _tilemap.WorldToCell(worldPosition);

            if (_tilemapUI.GetTile(_previousHoveredTile) == _hoverTileBase)
            {
                _tilemapUI.SetTile(_previousHoveredTile, null);
            }
            if (_tilemapUI.GetTile(cellPosition) != _selectTileBase)
                _tilemapUI.SetTile(cellPosition, _hoverTileBase);
            _previousHoveredTile = cellPosition;

            if (Input.GetMouseButtonDown(0) && _tilemap.GetTile(cellPosition) == null && !EventSystem.current.IsPointerOverGameObject())
            {
                _tileMapVisualizer.AddTile(cellPosition, TileType.Floor);
            }

            if (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace))
            {
                DeleteTiles();
            }
        }
        if (_cameraControl.IsDragging)
        {
            _tilemapUI.SetTile(_previousHoveredTile, null);

        }

    }

    public void DeleteTiles()
    {

        foreach (var tilePos in _previousClickedTiles)
        {
            if (_tilemapUI2.GetTile(tilePos) != _startTileBase)
            {
                _tilemap.SetTile(tilePos, null);
                _tilemapUI.SetTile(tilePos, null);
                _simpleMapGenerator.DeleteTile(tilePos);
            }
            _tilemapUI.SetTile(tilePos, null);
        }
        _previousClickedTiles.Clear();
        _startPosition.interactable = _previousClickedTiles.Count == 1;

    }

    public void SetStartPosition()
    {
        _tilemapUI2.SetTile(_simpleMapGenerator.GetStartPosition(), null);
        _simpleMapGenerator.SetStartPosition(_previousClickedTiles[0]);
        _tilemapUI2.SetTile(_previousClickedTiles[0], _startTileBase);
        _unBlockRun.Invoke();
    }
    public void ResetStartPosition()
    {
        _tilemapUI2.SetTile(_simpleMapGenerator.GetStartPosition(), null);
        _blockRun.Invoke();
    }

    public void ChangeTileType(int tileType)
    {
        foreach (var tile in _previousClickedTiles)
        {
            _simpleMapGenerator.SetTileType(tile, tileType);
            _tilemapUI.SetTile(tile, null);
        }
        _previousClickedTiles.Clear();
        _tilemapUI2.ClearAllTiles();
        _startPosition.interactable = _previousClickedTiles.Count == 1;


    }
}
