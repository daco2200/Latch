using System;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float shotCost = 5f;

    [Header("Reload (tune while playing)")]
    [SerializeField] private float reloadDelay = 0.8f;
    [SerializeField] private float overloadDelay = 0f;
    [SerializeField] private float reloadTime = 1.5f;

    public event Action ReloadStarted;
    public event Action ReloadCancelled;
    public event Action ReloadCompleted;
    public event Action OverloadStarted;

    public float Energy { get; private set; }
    public bool IsReloading { get; private set; }
    public bool IsOverloaded { get; private set; }
    public bool CanFire => !IsOverloaded && Energy >= shotCost;
    public int Bullets => Mathf.FloorToInt((Energy + 0.0001f) / shotCost);
    public int MaxBullets => Mathf.FloorToInt((maxEnergy + 0.0001f) / shotCost);
    public float ReloadProgress => IsReloading ? Mathf.Clamp01(reloadTimer / reloadTime) : 0f;

    private bool holding;
    private float waitTimer;
    private float reloadTimer;

    private void Awake()
    {
        Energy = maxEnergy;
        waitTimer = reloadDelay;
    }

    public void SetHolding(bool value) => holding = value;

    public bool TrySpend()
    {
        if (!CanFire) return false;

        if (IsReloading) CancelReload();

        Energy -= shotCost;
        waitTimer = reloadDelay;

        if (Bullets <= 0)
        {
            IsOverloaded = true;
            IsReloading = false;
            reloadTimer = 0f;
            waitTimer = overloadDelay;
            OverloadStarted?.Invoke();
        }
        return true;
    }

    private void LateUpdate()
    {
        if (IsOverloaded)
        {
            Recharge();
            return;
        }

        if (holding)
        {
            if (IsReloading) CancelReload();
            waitTimer = reloadDelay;
            return;
        }

        if (Energy < maxEnergy) Recharge();
    }

    private void Recharge()
    {
        if (!IsReloading)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                IsReloading = true;
                reloadTimer = 0f;
                ReloadStarted?.Invoke();
            }
            return;
        }

        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime)
        {
            Energy = maxEnergy;
            IsReloading = false;
            IsOverloaded = false;
            waitTimer = reloadDelay;
            ReloadCompleted?.Invoke();
        }
    }

    private void CancelReload()
    {
        IsReloading = false;
        reloadTimer = 0f;
        waitTimer = reloadDelay;
        ReloadCancelled?.Invoke();
    }
}