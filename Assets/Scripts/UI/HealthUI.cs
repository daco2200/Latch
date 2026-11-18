using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Image hullFill;   // Image Type = Filled, Horizontal
    [SerializeField] private Image lifeFill;

    private void Update()
    {
        hullFill.fillAmount = health.Hull01;
        lifeFill.fillAmount = health.Life01;
    }
}