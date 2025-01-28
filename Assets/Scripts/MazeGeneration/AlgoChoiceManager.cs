using System.Collections.Generic;
using UnityEngine;

public class AlgoChoiceManager : MonoBehaviour
{
   [SerializeField] private List<GameObject> AlgosUIParameters;
   int currentAlgo=0;

    public void changeAlgo(int index){
        AlgosUIParameters[currentAlgo].SetActive(false);
        currentAlgo=index;
        AlgosUIParameters[currentAlgo].SetActive(true);
    }
  
}
