using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    private Image _raycastBlocker;
    private void Awake() {
        _raycastBlocker=GetComponent<Image>();
        
    }
    public void DisplayPanel(GameObject panel){
        panel.SetActive(true);
        _raycastBlocker.raycastTarget=true;
    }

    public void UnDisplayPanel(GameObject panel){
        panel.SetActive(false);
        _raycastBlocker.raycastTarget=false;
    }
}
