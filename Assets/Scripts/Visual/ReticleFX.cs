using UnityEngine;
using UnityEngine.UI;

public class ReticleFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnergy energy;
    [SerializeField] private Transform[] arms;                // the four dashes
    [SerializeField] private Transform center;                // the central square
    [SerializeField] private SpriteRenderer[] spriteParts;    // parts to tint (sprites)
    [SerializeField] private Graphic[] uiParts;               // parts to tint (UI images), leave empty if unused
    [SerializeField] private LayerMask enemyMask;

    [Header("Center Sprite Swap")]
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private SpriteRenderer centerRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite firingSprite;
    [SerializeField] private float swapThreshold = 0.05f;

    [Header("Colors")]
    [SerializeField] private Color canShoot = Color.white;
    [SerializeField] private Color cannotShoot = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color onEnemy = new Color(0.9f, 0.76f, 0.35f);   // gold

    [Header("Oscillation (opens while firing)")]
    [SerializeField] private float bloomPerShot = 0.05f;
    [SerializeField] private float maxBloom = 0.35f;
    [SerializeField] private float bloomRecover = 1.2f;       // units per second
    [SerializeField] private float centerPulse = 0.2f;        // extra center scale at max bloom

    [Header("Hit marker (white X, child of the reticle)")]
    [SerializeField] private Transform hitMarker;
    [SerializeField] private float hitTime = 0.1f;
    [SerializeField] private float hitAngle = 4f;             // alternates +/- on each hit
    [SerializeField] private float hitPunch = 0.15f;

    [Header("Kill marker (gold, child of the reticle)")]
    [SerializeField] private Transform killMarker;
    [SerializeField] private float killTime = 0.25f;
    [SerializeField] private float killPunch = 0.4f;

    private Vector3[] rest;
    private Vector3 centerBase;
    private float bloom, hitTimer, killTimer;
    private int hitSign = 1;

    private void Awake()
    {
        rest = new Vector3[arms.Length];
        for (int i = 0; i < arms.Length; i++) rest[i] = arms[i].localPosition;
        centerBase = center.localScale;

        if (hitMarker != null) hitMarker.gameObject.SetActive(false);
        if (killMarker != null) killMarker.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerShoot.Fired += OnFired;
        EnemyLife.AnyHit += OnHit;
        EnemyLife.AnyDied += OnKill;     // not raised by the shockwave: only real kills
    }

    private void OnDisable()
    {
        PlayerShoot.Fired -= OnFired;
        EnemyLife.AnyHit -= OnHit;
        EnemyLife.AnyDied -= OnKill;
    }

    private void OnFired() => bloom = Mathf.Min(maxBloom, bloom + bloomPerShot);

    private void OnHit(EnemyLife enemy)
    {
        hitTimer = hitTime;
        hitSign = -hitSign;
    }

    private void OnKill(EnemyLife enemy) => killTimer = killTime;

    private void Update()
    {
        // Oscillation: opens with each shot and eases back when you stop
        bloom = Mathf.MoveTowards(bloom, 0f, bloomRecover * Time.deltaTime);
        for (int i = 0; i < arms.Length; i++)
            arms[i].localPosition = rest[i] + rest[i].normalized * bloom;
        float k = maxBloom > 0f ? bloom / maxBloom : 0f;
        center.localScale = centerBase * (1f + centerPulse * k);

        centerRenderer.sprite = playerShoot.IsHoldingShoot ? firingSprite : idleSprite;

        // State: gold = over enemy, gray = can't shoot, white = default
        bool blocked = !energy.CanFire || RuleState.WeaponJammed;
        bool overEnemy = Physics2D.OverlapPoint(transform.position, enemyMask) != null;

        Tint(overEnemy ? onEnemy : blocked ? cannotShoot : canShoot);

        // Markers
        UpdateMarker(hitMarker, ref hitTimer, hitTime, hitPunch, hitAngle * hitSign);
        UpdateMarker(killMarker, ref killTimer, killTime, killPunch, 0f);
    }

    private void Tint(Color c)
    {
        foreach (SpriteRenderer s in spriteParts) if (s != null) s.color = c;
        foreach (Graphic g in uiParts) if (g != null) g.color = c;
    }

    private static void UpdateMarker(Transform marker, ref float timer, float duration, float punch, float angle)
    {
        if (marker == null) return;

        if (timer > 0f) timer -= Time.deltaTime;
        bool active = timer > 0f;
        if (marker.gameObject.activeSelf != active) marker.gameObject.SetActive(active);
        if (!active) return;

        float k = timer / duration;                         // 1 at the start, 0 at the end
        marker.localScale = Vector3.one * (1f + punch * k);
        marker.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}