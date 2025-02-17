using System.Collections.Generic;
using UnityEngine;

public class AlgoChoiceManager : MonoBehaviour
{
   [SerializeField] private List<GameObject> _algosUIParameters;
   [SerializeField] private SimpleMapGenerator _simpleMapGenerator;
   int _currentAlgo=0;

    public void ChangeAlgo(int index){
        _algosUIParameters[_currentAlgo].SetActive(false);
        _currentAlgo=index;
        _algosUIParameters[_currentAlgo].SetActive(true);
        _simpleMapGenerator.SetIterations("0");
        _simpleMapGenerator.SetWalkLength("0");
        _simpleMapGenerator.SetProbalityOfCreateRoom("0");
    }
  
}
