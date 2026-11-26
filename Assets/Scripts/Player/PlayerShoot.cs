using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction shootAction;
    public static event System.Action Fired;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform pool;
    [SerializeField] private Transform pod;
    [SerializeField] private Transform pointer;
    [SerializeField] private PlayerEnergy energy;
    [SerializeField] private int bulletQuantity = 12;
    [SerializeField] private float fireInterval = 0.5f;

    private float cooldown;
    public bool IsHoldingShoot => shootAction != null && shootAction.IsPressed();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions.FindAction("Shoot");
    }

    private void Start()
    {
        int count = Mathf.Max(bulletQuantity, energy.MaxBullets);

        for (int i = 0; i < count; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, pool);
            bullet.SetActive(false);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        bool holding = shootAction.IsPressed();
        energy.SetHolding(holding);

        if (holding && energy.CanFire && !RuleState.WeaponJammed && cooldown <= 0f)
        {
            for (int i = 0; i < pool.childCount; i++)
            {
                GameObject bullet = pool.GetChild(i).gameObject;

                if (bullet.activeSelf == false)
                {
                    bullet.transform.position = pod.position;
                    Vector2 direction = ((Vector2)pointer.position - (Vector2)pod.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
                    cooldown += fireInterval;
                    bullet.SetActive(true);
                    Fired?.Invoke();
                    energy.TrySpend();
                    return;
                }
            }
        }
    }
}