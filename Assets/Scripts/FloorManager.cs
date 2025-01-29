using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorManager : MonoBehaviour
{

    [SerializeField] List<Floor> Floors;
    private static int currentFloor=0;
    private static GameObject Player;

    public void Awake()
    {
        Player=GameObject.FindGameObjectWithTag("Player");
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareTag("Exit"))
        {
            currentFloor++;
            Floors[currentFloor - 1].FloorComponents.SetActive(false);
            Player.transform.position = Floors[currentFloor].SpawnEntrance.position;
            Floors[currentFloor].FloorComponents.SetActive(true);

        }

        if (CompareTag("GoBack"))
        {
            currentFloor--;
            Floors[currentFloor + 1].FloorComponents.SetActive(false);
            Player.transform.position = Floors[currentFloor].SpawnEntrance.position;
            Floors[currentFloor].FloorComponents.SetActive(true);
        }


    }

}
