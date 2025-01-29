using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FloorsEditorManager : MonoBehaviour
{
    [SerializeField] private TileMapVisualizer tileMapVisualizer;
    [SerializeField] private TMP_InputField AddFloorInputField;
    [SerializeField] private Button SaveButton;
    [SerializeField] private GameObject TryToDeteLastLayerErrorText;
    private TMP_Dropdown dropdown;
    private int currentFloor = 0;
    private int previousFloor = 0;
    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        for (int i = 0; i < MazeData.mazeFloors.Count; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(MazeData.mazeFloors[i].name));
        }

        dropdown.RefreshShownValue(); // Mettre à jour l'affichage du dropdown

    }

    public void SetCurrentFloorSelected(int current)
    {
        previousFloor = currentFloor;
        currentFloor = current;
    }

    
    public int GetCurrentFloorSelected()
    {
        return currentFloor;
    }



    public void DeleteFloor()
    {
        if (dropdown.options.Count > 1)
        {
            dropdown.options.RemoveAt(currentFloor);
            if (currentFloor > 0)
            {
                MazeData.mazeFloors.RemoveAt(currentFloor);
                dropdown.onValueChanged.Invoke(currentFloor - 1);
                dropdown.RefreshShownValue();
            }
            else if (dropdown.options.Count > 0)
            {
                MazeData.mazeFloors.RemoveAt(currentFloor);
                dropdown.onValueChanged.Invoke(currentFloor);
                dropdown.RefreshShownValue();
            }
        }
        else
        {
            TryToDeteLastLayerErrorText.SetActive(true);
            ExecuteActionDelayed(new Action(() => TryToDeteLastLayerErrorText.SetActive(false)), 6000);
        }
    }

    async void ExecuteActionDelayed(Action a, int delayInMilliSeconds)
    {
        await Task.Delay(delayInMilliSeconds);
        a.Invoke();
    }


    public void SaveFloor()
    {
        MazeData.mazeFloors[currentFloor].name = dropdown.options[currentFloor].text;
        if (MazeData.mazeFloors[currentFloor].tileMap == null)
            MazeData.mazeFloors[currentFloor].tileMap = new();
        MazeData.mazeFloors[currentFloor].tileMap.Clear();
        MazeData.mazeFloors[currentFloor].tileMap = new(tileMapVisualizer.GetMapTiles().ToList());

        SetSaveButtonInteractability(false);
    }

    public void SetSaveButtonInteractability(bool value)
    {
        if (value != SaveButton.interactable)
            SaveButton.interactable = value;
    }

    public void AddFloor()
    {
        if (AddFloorInputField.text != "")
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(AddFloorInputField.text));
            MazeData.mazeFloors.Add(new FloorData(){name=AddFloorInputField.text, entreeSorties=new(),tileMap=new()});
        }
        else
        {
            //clignoter new border pendant X secondes
        }
    }




}
