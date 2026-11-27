
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoReadout : MonoBehaviour
{
    [SerializeField] private PlayerEnergy energy;
    [SerializeField] private ReticleFX reticle;
    [SerializeField] private Camera cam;

    [Header("Parts")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private GameObject reloadBar;
    [SerializeField] private Image reloadBG;
    [SerializeField] private Image reloadFill;

    [Header("Ammo Colors")]
    [SerializeField] private Color normalAmmoColor = new Color32(217, 217, 217, 255);
    [SerializeField] private Color fullAmmoColor = new Color(0.9f, 0.76f, 0.35f);
    [SerializeField] private Color blockedAmmoColor = new Color(0.5f, 0.5f, 0.5f);

    [Header("Reload FX")]
    [SerializeField] private Color completionColor = new Color(0.9f, 0.76f, 0.35f);
    [SerializeField] private Color cancelColor = new Color(0.9f, 0.25f, 0.25f);
    [SerializeField] private float completionDuration = 0.35f;
    [SerializeField] private float cancelDuration = 0.15f;

    [Header("Layout (mirrors with the side)")]
    [SerializeField] private float numberOffset = 0.9f;
    [SerializeField] private float barOffset = 2.0f;
    [SerializeField] private float flipMargin = 0.12f;

    private float originalNumberX;
    private float originalBarX;

    private Color originalFillColor;
    private Color originalBGColor;

    private float fxTimer;
    private bool reloadFX;
    private bool completed;
    private bool ammoFullHighlight;

    private int previousBullets;

    private void Awake()
    {
        originalNumberX = ammoText.transform.localPosition.x;
        originalBarX = reloadBar.transform.localPosition.x;

        originalFillColor = reloadFill.color;
        originalBGColor = reloadBG.color;

        ammoFullHighlight = false;
        previousBullets = energy.Bullets;

        ammoText.color = normalAmmoColor;

        reloadBar.SetActive(false);
    }

    private void OnEnable()
    {
        energy.ReloadStarted += OnReloadStarted;
        energy.ReloadCompleted += OnReloadCompleted;
        energy.ReloadCancelled += OnReloadCancelled;
    }

    private void OnDisable()
    {
        energy.ReloadStarted -= OnReloadStarted;
        energy.ReloadCompleted -= OnReloadCompleted;
        energy.ReloadCancelled -= OnReloadCancelled;
    }

    private void OnReloadStarted()
    {
        ammoFullHighlight = false;

        reloadFX = false;
        completed = false;

        reloadBar.SetActive(true);
        reloadFill.fillAmount = 0f;
        reloadFill.color = originalFillColor;
        reloadBG.color = originalBGColor;
    }

    private void OnReloadCompleted()
    {
        ammoFullHighlight = true;
        StartReloadFX(completionColor, completionDuration, true);
    }

    private void OnReloadCancelled()
    {
        StartReloadFX(cancelColor, cancelDuration, false);
    }

    private void StartReloadFX(Color color, float duration, bool success)
    {
        reloadBar.SetActive(true);

        reloadFX = true;
        completed = success;

        fxTimer = duration;

        if (success)
        {
            reloadFill.fillAmount = 1f;
            reloadBG.color = originalBGColor;
        }
        else
        {
            reloadBG.color = cancelColor;
        }

        reloadFill.color = color;
    }

    private void Update()
    {
        ammoText.text = energy.Bullets.ToString("00");

        int currentBullets = energy.Bullets;

        if (currentBullets < previousBullets)
            ammoFullHighlight = false;

        previousBullets = currentBullets;

        bool blocked = !energy.CanFire || RuleState.WeaponJammed;

        ammoText.color = blocked
            ? blockedAmmoColor
            : ammoFullHighlight && currentBullets == energy.MaxBullets
                ? fullAmmoColor
                : normalAmmoColor;

        // Reload bar
        if (energy.IsReloading)
        {
            reloadBar.SetActive(true);

            reloadFill.fillAmount = energy.ReloadProgress;
            reloadFill.color = originalFillColor;
        }
        else if (reloadFX)
        {
            fxTimer -= Time.deltaTime;

            if (fxTimer <= 0f)
            {
                reloadFX = false;
                reloadBar.SetActive(false);

                reloadFill.color = originalFillColor;
                reloadBG.color = originalBGColor;
            }
        }
        else if (!completed)
        {
            reloadBar.SetActive(false);
        }

        // Left of the reticle by default; near the left edge it mirrors to the right
        float vx = cam.WorldToViewportPoint(transform.position).x;
        bool flipped = vx < flipMargin;

        SetX(ammoText.transform, originalNumberX + (flipped ? numberOffset : 0f));
        SetX(reloadBar.transform, originalBarX + (flipped ? barOffset : 0f));

        ammoText.alignment = flipped
            ? TextAlignmentOptions.MidlineLeft
            : TextAlignmentOptions.MidlineRight;
    }

    private static void SetX(Transform t, float x)
    {
        Vector3 p = t.localPosition;
        p.x = x;
        t.localPosition = p;
    }
}
