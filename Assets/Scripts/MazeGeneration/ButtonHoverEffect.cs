using UnityEngine;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour
{
    RectTransform _buttonTransform; // Le RectTransform du bouton
    [SerializeField] private Vector3 _hoverScale = new Vector3(1.2f, 1.2f, 1f); // Échelle au survol
    [SerializeField] private float _animationSpeed = 0.2f; // Vitesse d'animation
    Button _button;

    private Vector3 _originalScale;

    private void Start()
    {
        _buttonTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        // Stocker l'échelle initiale
        _originalScale = _buttonTransform.localScale;

    }

    public void OnPointerEnter()
    {
        if (_button.IsInteractable())
        {
            StopAllCoroutines();
            StartCoroutine(ScaleButton(_hoverScale));
        }
    }

    public void OnPointerExit()
    {
        if (_button.IsInteractable())
        {
            StopAllCoroutines();
            StartCoroutine(ScaleButton(_originalScale));
        }
    }

    private System.Collections.IEnumerator ScaleButton(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = _buttonTransform.localScale;

        while (elapsedTime < _animationSpeed)
        {
            _buttonTransform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / _animationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _buttonTransform.localScale = targetScale;
    }
}
