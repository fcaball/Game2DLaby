using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    private Image raycastBlocker;
    private void Start() {
        raycastBlocker=GetComponent<Image>();
        
    }
    public void DisplayPanel(GameObject panel){
        panel.SetActive(true);
        raycastBlocker.raycastTarget=true;
    }

    public void UnDisplayPanel(GameObject panel){
        panel.SetActive(false);
        raycastBlocker.raycastTarget=false;
    }
}
