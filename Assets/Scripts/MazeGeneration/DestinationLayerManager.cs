using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DestinationLayerManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button _addInOutButton;
    [SerializeField] private PointerClickGrid _pointerClickGrid;
    [SerializeField] private MazeManager _mazeDrawer;
    [SerializeField] private Tilemap _tilemapForeground;
    [SerializeField] private Tilemap _tilemapBackground;
    [SerializeField] private TileBase _tileSelected;
    Vector3Int _positionDestinationSpawn;
    private void Start()
    {
        _positionDestinationSpawn = new Vector3Int(-1, -1, -1);
    }
    private void OnEnable()
    {
        _dropdown.ClearOptions();

        foreach (var layer in MazeData.MazeFloors)
        {
            _dropdown.options.Add(new(layer.Name));
        }
        _dropdown.RefreshShownValue();
        _addInOutButton.interactable = false;
        _mazeDrawer.gameObject.SetActive(true);
        _mazeDrawer.SimpleLoadFloor(_dropdown.value);
    }

    public void AddInOut()
    {
        _pointerClickGrid.SetInOut(_dropdown.value, _positionDestinationSpawn);
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            bool IsOnUi = false;
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI") && result.gameObject.name != "Canvas PopUps")
                {
                    IsOnUi = true;
                }
            }
            if (!IsOnUi)
            {

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                Vector3Int cellPosition = _tilemapForeground.WorldToCell(worldPos);
                if (_tilemapBackground.GetTile(cellPosition) != null && MazeData.MazeFloors[_dropdown.value].TileInfos.Find((t) => t.Position == cellPosition).Tag == TileTag.Floor)
                {
                    List<InOut> Destinations = MazeData.MazeFloors
            .SelectMany(e => e.EntreeSorties.Where(io => io.IndexFloorDestination == _dropdown.value))
            .ToList();
                    if (MazeData.MazeFloors[_dropdown.value].Minerals.Contains(cellPosition) || MazeData.MazeFloors[_dropdown.value].EntreeSorties.Find(io => io.Location == cellPosition) == null || Destinations.Find(d => d.SpawnInDestination == cellPosition) == null)
                    {
                        _tilemapForeground.ClearAllTiles();
                        if (_positionDestinationSpawn == new Vector3Int(-1, -1, -1))
                        {

                            _tilemapForeground.SetTile(cellPosition, _tileSelected);
                            _positionDestinationSpawn = cellPosition;

                        }

                        if (_positionDestinationSpawn != cellPosition)
                        {
                            _mazeDrawer.SimpleLoadFloor(_dropdown.value);
                            _tilemapForeground.SetTile(cellPosition, _tileSelected);
                            _positionDestinationSpawn = cellPosition;
                        }
                        _addInOutButton.interactable = _tilemapForeground.GetUsedTilesCount() == 1;
                    }
                }
            }
        }
    }






}
