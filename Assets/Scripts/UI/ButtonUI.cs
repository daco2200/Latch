
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text buttonText;

    [Header("Hover Settings")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private TMP_FontAsset hoverFont;

    private Color originalBackgroundColor;
    private Color originalTextColor;
    private TMP_FontAsset originalFont;

    private void Awake()
    {
        if (background == null)
            background = GetComponent<Image>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        if (background != null)
            originalBackgroundColor = background.color;

        if (buttonText != null)
        {
            originalTextColor = buttonText.color;
            originalFont = buttonText.font;
        }
    }

    private void OnEnable()
    {
        ApplyNormalState();
    }

    private void OnDisable()
    {
        ApplyNormalState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyHoverState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyNormalState();
    }

    private void ApplyHoverState()
    {
        if (background != null)
            background.color = hoverColor;

        if (buttonText != null)
        {
            buttonText.color = hoverColor;

            if (hoverFont != null)
                buttonText.font = hoverFont;
        }
    }

    private void ApplyNormalState()
    {
        if (background != null)
            background.color = originalBackgroundColor;

        if (buttonText != null)
        {
            buttonText.color = originalTextColor;
            buttonText.font = originalFont;
        }
    }
}
