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
    [SerializeField] private TileMapVisualizer _tileMapVisualizer;
    [SerializeField] private TMP_InputField _addFloorInputField;
    [SerializeField] private Button _saveButton;
    [SerializeField] private GameObject _tryToDeteLastLayerErrorText;
    private TMP_Dropdown _dropdown;
    private int _currentFloor = 0;
    private int _previousFloor = 0;

    private void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.ClearOptions();
        for (int i = 0; i < MazeData.MazeFloors.Count; i++)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData(MazeData.MazeFloors[i].Name));
        }

        _dropdown.RefreshShownValue(); // Mettre à jour l'affichage du dropdown

    }

  private void Update() {
    // Vérifie si Command (ou Control) est enfoncé avec la touche S
    if ((Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S)) {
        Debug.Log("saved");
        SaveFloor();
    }
}


    public void SetCurrentFloorSelected(int current)
    {
        _previousFloor = _currentFloor;
        _currentFloor = current;
    }

    
    public int GetCurrentFloorSelected()
    {
        return _currentFloor;
    }



    public void DeleteFloor()
    {
        if (_dropdown.options.Count > 1)
        {
            _dropdown.options.RemoveAt(_currentFloor);
            if (_currentFloor > 0)
            {
                MazeData.MazeFloors.RemoveAt(_currentFloor);
                _dropdown.onValueChanged.Invoke(_currentFloor - 1);
                _dropdown.RefreshShownValue();
            }
            else if (_dropdown.options.Count > 0)
            {
                MazeData.MazeFloors.RemoveAt(_currentFloor);
                _dropdown.onValueChanged.Invoke(_currentFloor);
                _dropdown.RefreshShownValue();
            }
        }
        else
        {
            _tryToDeteLastLayerErrorText.SetActive(true);
            ExecuteActionDelayed(new Action(() => _tryToDeteLastLayerErrorText.SetActive(false)), 6000);
        }
    }

    async void ExecuteActionDelayed(Action a, int delayInMilliSeconds)
    {
        await Task.Delay(delayInMilliSeconds);
        a.Invoke();
    }


    public void SaveFloor()
    {
        MazeData.MazeFloors[_currentFloor].Name = _dropdown.options[_currentFloor].text;
        if (MazeData.MazeFloors[_currentFloor].TileMap == null)
            MazeData.MazeFloors[_currentFloor].TileMap = new();
        MazeData.MazeFloors[_currentFloor].TileMap.Clear();
        MazeData.MazeFloors[_currentFloor].TileMap = new(_tileMapVisualizer.GetMapTiles().ToList());

        SetSaveButtonInteractability(false);
    }

    public void SetSaveButtonInteractability(bool value)
    {
        _saveButton.interactable = value;
        Debug.Log(value);
    }

    public void AddFloor()
    {
        if (_addFloorInputField.text != "")
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_addFloorInputField.text));
            MazeData.MazeFloors.Add(new FloorData(){Name=_addFloorInputField.text, EntreeSorties=new(),TileMap=new()});
        }
        else
        {
            //clignoter new border pendant X secondes
        }
    }




}
