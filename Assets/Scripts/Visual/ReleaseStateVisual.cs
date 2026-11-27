using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReleaseStateVisual : MonoBehaviour
{
    [SerializeField] private ReleaseBar bar;
    [SerializeField] private TMP_Text label;                 // "SHIFT"
    [SerializeField] private Graphic[] uiParts;              // frame/fill to tint (UI), optional
    [SerializeField] private SpriteRenderer[] spriteParts;   // frame/fill to tint (sprites), optional

    [Header("Filling")]
    [SerializeField] private Color fillingColor = new Color(1f, 1f, 1f, 0.45f);
    [SerializeField] private TMP_FontAsset fillingFont;      // Manrope Regular

    [Header("Full (ready)")]
    [SerializeField] private Color fullColor = new Color(0.9f, 0.76f, 0.35f);
    [SerializeField] private TMP_FontAsset fullFont;         // Manrope Bold
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseTime = 0.25f;

    private bool wasFull;
    private float pulseTimer;
    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = label.transform.localScale;
        Apply(false);
    }

    private void Update()
    {
        bool full = bar.IsFull;
        if (full != wasFull)
        {
            wasFull = full;
            Apply(full);
            if (full) pulseTimer = pulseTime;
        }

        if (pulseTimer > 0f)
        {
            pulseTimer -= Time.deltaTime;
            float k = Mathf.Max(0f, pulseTimer / pulseTime);
            label.transform.localScale = baseScale * (1f + (pulseScale - 1f) * k);
        }
    }

    private void Apply(bool full)
    {
        Color c = full ? fullColor : fillingColor;
        TMP_FontAsset font = full ? fullFont : fillingFont;

        if (font != null) label.font = font;
        label.color = c;
        foreach (Graphic g in uiParts) if (g != null) g.color = c;
        foreach (SpriteRenderer s in spriteParts) if (s != null) s.color = c;
    }
}