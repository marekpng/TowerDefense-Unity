using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage; // Assumes button has Image component for background

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;
        originalColor = buttonImage.color; // Save starting color (dark gray)
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * 1.1f; // Scale up
        buttonImage.color = new Color(0.33f, 0.33f, 0.8f); // Blue glow (#5555CC)
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // Back to normal
        buttonImage.color = originalColor; // Back to dark
    }
}