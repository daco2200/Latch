using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction shootAction;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform pool;
    [SerializeField] private Transform pod;
    [SerializeField] private Transform pointer;
    [SerializeField] private int bulletQuantity = 12;
    [SerializeField] private float fireInterval = 0.5f;

    private float cooldown;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions.FindAction("Shoot");
    }

    private void Start()
    {
        for (int i = 0; i < bulletQuantity; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, pool);
            bullet.SetActive(false);
        }
    }

    private void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;
        if (shootAction.IsPressed() && cooldown <= 0f)
        {
            for (int i = 0;i < pool.childCount; i++)
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
                    return;
                }
            }
        }
    }
}
