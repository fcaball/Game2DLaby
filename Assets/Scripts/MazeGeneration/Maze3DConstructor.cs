using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze3DConstructor : MonoBehaviour
{
    [SerializeField] private GameObject _floor;
    private List<GameObject> _floors = new();
    [SerializeField] private GameObject _wall;
    private List<GameObject> _walls = new();
    private void OnEnable()
    {
        List<TileDatas> tiles = MazeData.MazeFloors[MazeData.CurrentFloor].TileInfos;
        foreach (TileDatas tile in tiles)
        {
            if (tile.Tag == TileTag.Floor)
            {

                GameObject p = _floors.FirstOrDefault(f => !f.activeSelf);
                if (p == null)
                {
                    Debug.Log("Instantiate Floor");
                    p = Instantiate(_floor);
                    _floors.Add(p);
                }
                p.SetActive(true);
                p.transform.localPosition = new Vector3(tile.Position.x, tile.Position.z, tile.Position.y);
                p.transform.parent = transform;
            }
            else
            {
                GameObject w = _walls.FirstOrDefault(w => !w.activeSelf);
                if (w == null)
                {
                    Debug.Log("Instantiate Wall");
                    w = Instantiate(_wall);
                    _walls.Add(w);
                }
                w.SetActive(true);
                w.transform.localPosition = new Vector3(tile.Position.x, tile.Position.z + 1, tile.Position.y);
                w.transform.parent = transform;

                if (MazeData.MazeFloors[MazeData.CurrentFloor].EntreeSorties.Find(io => io.Location == tile.Position) != null)
                {
                    BoxCollider boxCollider=w.GetComponent<BoxCollider>();
                    boxCollider.isTrigger = true;
                    
                }
            }

        }
    }
    void OnDisable()
    {
        foreach (var floor in _floors)
        {
            floor.SetActive(false);
        }

        foreach (var wall in _walls)
        {
            wall.SetActive(false);
        }
    }
}
