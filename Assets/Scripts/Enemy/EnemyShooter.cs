using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private BulletPattern[] patterns;    // one is picked at random per enemy
    [SerializeField] private Vector2 firstDelayRange = new Vector2(0.3f, 2f);

    private EnemyMover mover;
    private EnemyBulletPool pool;
    private PatternContext context;
    private BulletPattern pattern;
    private float interval, timer;
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

        pattern = patterns[Random.Range(0, patterns.Length)];
        interval = pattern.cooldown * Random.Range(0.8f, 1.25f);
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

    private void Update()
    {
        if (!mover.Arrived || pool == null || firing) return;   // only shoots once stopped

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        StartCoroutine(Run());
        timer += interval * GameFlow.IntervalScale;
    }

    private IEnumerator Run()
    {
        firing = true;
        yield return pattern.Execute(context);
        firing = false;
    }
}