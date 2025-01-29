using System.Collections.Generic;
using UnityEngine;

public class AlgoChoiceManager : MonoBehaviour
{
   [SerializeField] private List<GameObject> AlgosUIParameters;
   [SerializeField] private SimpleMapGenerator simpleMapGenerator;
   int currentAlgo=0;

    public void changeAlgo(int index){
        AlgosUIParameters[currentAlgo].SetActive(false);
        currentAlgo=index;
        AlgosUIParameters[currentAlgo].SetActive(true);
        simpleMapGenerator.SetIterations("0");
        simpleMapGenerator.SetWalkLength("0");
        simpleMapGenerator.SetProbalityOfCreateRoom("0");
    }
  
}
