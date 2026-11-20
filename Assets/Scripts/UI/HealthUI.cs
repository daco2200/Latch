using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Image hullFill;              // Filled, Horizontal
    [SerializeField] private Image lifeFill;              // Filled, Horizontal
    [SerializeField] private RectTransform corruptionRect;

    private static float Safe01(float v) =>
        float.IsNaN(v) || float.IsInfinity(v) ? 0f : Mathf.Clamp01(v);

    private void Update()
    {
        float life = Safe01(health.Life01);
        float corruption = Safe01(health.Corruption01);

        hullFill.fillAmount = Safe01(health.Hull01);
        lifeFill.fillAmount = life;

        bool show = corruption > 0f;
        corruptionRect.gameObject.SetActive(show);
        if (!show) return;

        float from = Mathf.Max(0f, life - corruption);
        corruptionRect.anchorMin = new Vector2(from, 0f);
        corruptionRect.anchorMax = new Vector2(life, 1f);
        corruptionRect.offsetMin = Vector2.zero;
        corruptionRect.offsetMax = Vector2.zero;
    }
}