using UnityEngine;

public class CameraControl : MonoBehaviour
{


    private int multiplicateurCameraSpeed = 1;
    private int[] multiplicators = { 1, 2, 4 };
    [SerializeField] Grid grid;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * Time.deltaTime * 2 * multiplicateurCameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.up * Time.deltaTime * 2 * multiplicateurCameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * Time.deltaTime * 2 * multiplicateurCameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * 2 * multiplicateurCameraSpeed;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0 && (grid.transform.localScale.x-(1 * Time.deltaTime * 12 * multiplicateurCameraSpeed))>0.15) // Molette vers le bas
        {
            grid.transform.localScale -= Vector3.one * Time.deltaTime * 12 * multiplicateurCameraSpeed;
        }
        else if (scroll > 0 && (grid.transform.localScale.x-(1 * Time.deltaTime * 12 * multiplicateurCameraSpeed))<4) // Molette vers le haut
        {
            grid.transform.localScale += Vector3.one * Time.deltaTime * 12 * multiplicateurCameraSpeed;
        }
    }
    public void SetMultiplicateurCameraSpeed(int multiplicateur)
    {
        multiplicateurCameraSpeed = multiplicators[multiplicateur];
    }
}
