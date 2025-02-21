using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{


    private int _multiplicateurCameraSpeed = 1;
    private int[] _multiplicators = { 1, 2, 4 };
    [SerializeField] Grid _grid;
    [SerializeField] private Image _imageBlocker;
    [SerializeField] private Texture2D _moveCursor;

    private bool _isDragging = false;
    public bool IsDragging => _isDragging;

    private Vector3 _lastMousePosition;

   
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
        if (scroll < 0 && (_grid.transform.localScale.x - (1 * Time.deltaTime * 12 * _multiplicateurCameraSpeed)) > 0.15) // Molette vers le bas
        {
            _grid.transform.localScale -= Vector3.one * Time.deltaTime * 12 * _multiplicateurCameraSpeed;
        }
        else if (scroll > 0 && (_grid.transform.localScale.x - (1 * Time.deltaTime * 12 * _multiplicateurCameraSpeed)) < 4) // Molette vers le haut
        {
            _grid.transform.localScale += Vector3.one * Time.deltaTime * 12 * _multiplicateurCameraSpeed;
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
         
        }

        if (_isDragging && Input.GetMouseButton(1))
        {
               Cursor.SetCursor(_moveCursor, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true; 
            Vector3 deltaMouse = Input.mousePosition - _lastMousePosition;
            transform.position -= Vector3.right * deltaMouse.x * Time.deltaTime * _multiplicateurCameraSpeed;
            transform.position -= Vector3.up * deltaMouse.y * Time.deltaTime * _multiplicateurCameraSpeed;
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isDragging = false;
             Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true; 
        }
    }
    public void SetMultiplicateurCameraSpeed(int multiplicateur)
    {
        _multiplicateurCameraSpeed = _multiplicators[multiplicateur];
    }
}
