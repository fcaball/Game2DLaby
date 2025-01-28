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
    private Tilemap tilemap; // Référence à la Tilemap
    [SerializeField] private Tilemap tilemapUI; // Référence à la Tilemap
    [SerializeField] private Tilemap tilemapUI2; // Référence à la Tilemap
    [SerializeField] private TileMapVisualizer tileMapVisualizer;
    [SerializeField] private SimpleMapGenerator simpleMapGenerator;
    [SerializeField] private Button startPosition;

    [SerializeField] private TileBase selectTileBase;
    [SerializeField] private TileBase startTileBase;
    [SerializeField] private TileBase hoverTileBase;
    private List<Vector3Int> previousClickedTiles = new();
    private Vector3Int previousHoveredTile;
    [SerializeField] private UnityEvent BlockRun;
    [SerializeField] private UnityEvent UnBlockRun;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = 0;
        tilemapUI.GetComponent<TilemapRenderer>().sortingOrder = 1;
        tilemapUI2.GetComponent<TilemapRenderer>().sortingOrder = 2;
    }

   
    public void OnPointerClick(PointerEventData eventData)
    {
        // Convertir la position du clic de la souris en coordonnées du monde
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = tilemap.transform.position.z; // Ajuster la profondeur Z

        // Convertir les coordonnées du monde en coordonnées de cellule
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        OnClickTile(cellPosition);

    }

    public void OnClickTile(Vector3Int cellPosition)
    {
        if (tilemapUI.GetTile(cellPosition) == selectTileBase &&
         (Input.GetKey(KeyCode.LeftControl) || (!Input.GetKey(KeyCode.LeftControl) && previousClickedTiles.Count == 1)))
        {
            tilemapUI.SetTile(cellPosition, null);
            previousClickedTiles.Remove(cellPosition);
        }
        else
        {

            if (!Input.GetKey(KeyCode.LeftControl))
            {
                foreach (var position in previousClickedTiles)
                {
                    tilemapUI.SetTile(position, null);
                }
                previousClickedTiles.Clear();
            }
            tilemapUI.SetTile(cellPosition, selectTileBase);
            previousClickedTiles.Add(cellPosition);
        }
        startPosition.interactable = previousClickedTiles.Count == 1;
    }

    private void Update()
    {

        // //  Convertir la position du clic de la souris en coordonnées du monde
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = tilemap.transform.position.z; // Ajuster la profondeur Z

        // // Convertir les coordonnées du monde en coordonnées de cellule
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (tilemapUI.GetTile(previousHoveredTile) == hoverTileBase)
        {
            tilemapUI.SetTile(previousHoveredTile, null);
        }
        if (tilemapUI.GetTile(cellPosition) != selectTileBase)
            tilemapUI.SetTile(cellPosition, hoverTileBase);
        previousHoveredTile = cellPosition;

        if (Input.GetMouseButtonDown(0) && tilemap.GetTile(cellPosition) == null && !EventSystem.current.IsPointerOverGameObject())
        {
            tileMapVisualizer.AddTile(cellPosition, TileType.Floor);
        }

        if (Input.GetKey(KeyCode.Delete))
        {
            DeleteTiles();
        }
    }

    public void DeleteTiles()
    {

        foreach (var tilePos in previousClickedTiles)
        {
            if (tilemapUI2.GetTile(tilePos) != startTileBase)
            {
                tilemap.SetTile(tilePos, null);
                tilemapUI.SetTile(tilePos, null);
                simpleMapGenerator.DeleteTile(tilePos);
            }
            tilemapUI.SetTile(tilePos, null);
        }
        previousClickedTiles.Clear();
        startPosition.interactable = previousClickedTiles.Count == 1;

    }

    public void SetStartPosition()
    {
        tilemapUI2.SetTile(simpleMapGenerator.GetStartPosition(), null);
        simpleMapGenerator.SetStartPosition(previousClickedTiles[0]);
        tilemapUI2.SetTile(previousClickedTiles[0], startTileBase);
        UnBlockRun.Invoke();
    }
    public void ResetStartPosition()
    {
        tilemapUI2.SetTile(simpleMapGenerator.GetStartPosition(), null);
        BlockRun.Invoke();
    }

    public void ChangeTileType(int tileType)
    {
        foreach (var tile in previousClickedTiles)
        {
            simpleMapGenerator.SetTileType(tile, tileType);
            tilemapUI.SetTile(tile, null);
        }
        previousClickedTiles.Clear();
        startPosition.interactable = previousClickedTiles.Count == 1;


    }
}
