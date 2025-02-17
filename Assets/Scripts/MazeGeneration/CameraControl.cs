using UnityEngine;

public class CameraControl : MonoBehaviour
{


    private int _multiplicateurCameraSpeed = 1;
    private int[] _multiplicators = { 1, 2, 4 };
    [SerializeField] Grid _grid;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * Time.deltaTime * 2 * _multiplicateurCameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.up * Time.deltaTime * 2 * _multiplicateurCameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * Time.deltaTime * 2 * _multiplicateurCameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * 2 * _multiplicateurCameraSpeed;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0 && (_grid.transform.localScale.x-(1 * Time.deltaTime * 12 * _multiplicateurCameraSpeed))>0.15) // Molette vers le bas
        {
            _grid.transform.localScale -= Vector3.one * Time.deltaTime * 12 * _multiplicateurCameraSpeed;
        }
        else if (scroll > 0 && (_grid.transform.localScale.x-(1 * Time.deltaTime * 12 * _multiplicateurCameraSpeed))<4) // Molette vers le haut
        {
            _grid.transform.localScale += Vector3.one * Time.deltaTime * 12 * _multiplicateurCameraSpeed;
        }
    }
    public void SetMultiplicateurCameraSpeed(int multiplicateur)
    {
        _multiplicateurCameraSpeed = _multiplicators[multiplicateur];
    }
}
