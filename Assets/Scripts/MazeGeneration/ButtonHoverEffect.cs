using UnityEngine;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour
{
    RectTransform buttonTransform; // Le RectTransform du bouton
    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f); // Échelle au survol
    [SerializeField] private float animationSpeed = 0.2f; // Vitesse d'animation
    Button button;

    private Vector3 originalScale;

    private void Start()
    {
        buttonTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        // Stocker l'échelle initiale
        originalScale = buttonTransform.localScale;

    }

    public void OnPointerEnter()
    {
        if (button.IsInteractable())
        {
            StopAllCoroutines();
            StartCoroutine(ScaleButton(hoverScale));
        }
    }

    public void OnPointerExit()
    {
        if (button.IsInteractable())
        {
            StopAllCoroutines();
            StartCoroutine(ScaleButton(originalScale));
        }
    }

    private System.Collections.IEnumerator ScaleButton(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = buttonTransform.localScale;

        while (elapsedTime < animationSpeed)
        {
            buttonTransform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / animationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buttonTransform.localScale = targetScale;
    }
}
