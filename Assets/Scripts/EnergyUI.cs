using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private PlayerEnergy energy;
    [SerializeField] private TMP_Text bulletsText;
    [SerializeField] private GameObject reloadBar;
    [SerializeField] private Image reloadFill;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color overloadColor = Color.red;

    private void Update()
    {
        Color c = energy.IsOverloaded ? overloadColor : normalColor;

        bulletsText.text = energy.Bullets.ToString();
        bulletsText.color = c;

        reloadBar.SetActive(energy.IsReloading);
        if (energy.IsReloading)
        {
            reloadFill.fillAmount = energy.ReloadProgress;
            reloadFill.color = c;
        }
    }
}