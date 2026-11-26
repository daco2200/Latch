using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private Transform origin;              // the player
    [SerializeField] private EnemyBulletPool bulletPool;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform visual;              // ring sprite of 1 unit of diameter (optional)
    [SerializeField] private float speed = 25f;             // units per second
    [SerializeField] private float maxRadius = 30f;

    [Header("Triggers")]
    [SerializeField] private bool onPulled = true;
    [SerializeField] private bool onWon = true;
    [SerializeField] private bool onTimedOut = true;

    private float radius;
    private Vector2 center;
    private bool active;

    private void OnEnable() => GameFlow.ChallengeEnded += OnChallengeEnded;
    private void OnDisable() => GameFlow.ChallengeEnded -= OnChallengeEnded;

    private void Start()
    {
        if (visual != null) visual.gameObject.SetActive(false);
    }

    private void OnChallengeEnded(ChallengeResult result)
    {
        bool trigger = (result == ChallengeResult.Pulled && onPulled)
                    || (result == ChallengeResult.Won && onWon)
                    || (result == ChallengeResult.TimedOut && onTimedOut);
        if (!trigger) return;

        radius = 0f;
        center = origin.position;
        active = true;

        if (visual != null)
        {
            visual.position = center;
            visual.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!active) return;

        radius += speed * Time.deltaTime;

        foreach (Collider2D c in Physics2D.OverlapCircleAll(center, radius, enemyMask))
        {
            EnemyLife enemy = c.GetComponent<EnemyLife>();
            if (enemy != null) enemy.Dissolve();
        }
        bulletPool.ClearWithin(center, radius);

        if (visual != null) visual.localScale = Vector3.one * radius * 2f;

        if (radius >= maxRadius)
        {
            active = false;
            if (visual != null) visual.gameObject.SetActive(false);
        }
    }
}