using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private BulletPattern[] patterns;
    [SerializeField] private bool changePatternEachShot;      // ON for the boss
    [SerializeField] private Vector2 firstDelayRange = new Vector2(0.3f, 2f);

    private EnemyMover mover;
    private EnemyBulletPool pool;
    private PatternContext context;
    private BulletPattern pattern;
    private float jitter, timer;
    private bool firing;

    private void Awake()
    {
        mover = GetComponent<EnemyMover>();

        if (patterns == null || patterns.Length == 0)
        {
            Debug.LogWarning("EnemyShooter: no patterns assigned", this);
            enabled = false;
            return;
        }

        pattern = Pick();
        jitter = Random.Range(0.8f, 1.25f);
        timer = Random.Range(firstDelayRange.x, firstDelayRange.y);

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        context = new PatternContext
        {
            source = transform,
            target = player != null ? player.transform : null
        };
    }

    public void Init(EnemyBulletPool bulletPool)
    {
        pool = bulletPool;
        if (context != null) context.pool = bulletPool;
    }

    private BulletPattern Pick() => patterns[Random.Range(0, patterns.Length)];

    private void Update()
    {
        if (!mover.Arrived || pool == null || firing) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (changePatternEachShot) pattern = Pick();
        StartCoroutine(Run());
        timer += pattern.cooldown * jitter * GameFlow.IntervalScale;
    }

    private IEnumerator Run()
    {
        firing = true;
        yield return pattern.Execute(context);
        firing = false;
    }
}