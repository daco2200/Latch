using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private Vector2 intervalRange = new Vector2(1.5f, 3.5f);
    [SerializeField] private Vector2 firstDelayRange = new Vector2(0.3f, 2f);
    [SerializeField] private float spread = 12f;   // degrees, random around the player direction

    private EnemyMover mover;
    private EnemyBulletPool pool;
    private Transform target;
    private float interval, timer;

    private void Awake()
    {
        mover = GetComponent<EnemyMover>();
        interval = Random.Range(intervalRange.x, intervalRange.y);
        timer = Random.Range(firstDelayRange.x, firstDelayRange.y);

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null) target = player.transform;
        else Debug.LogWarning("EnemyShooter: no PlayerHealth found in the scene");
    }

    public void Init(EnemyBulletPool bulletPool) => pool = bulletPool;

    private void Update()
    {
        if (!mover.Arrived || pool == null || target == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Vector2 toPlayer = (target.position - transform.position).normalized;
        Vector2 dir = Quaternion.Euler(0f, 0f, Random.Range(-spread, spread)) * toPlayer;
        pool.Fire(transform.position, dir);
        timer += interval;
    }
}