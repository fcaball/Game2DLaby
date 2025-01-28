using System.Collections.Generic;
using UnityEngine;

public class Floor:MonoBehaviour
{
    public GameObject FloorComponents;
    public Transform SpawnEntrance;
    public BoxCollider2D GoBackZone;
    public List<BoxCollider2D> Exits;
}
