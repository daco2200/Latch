using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action PlayerDied;

    [Header("Hull (regenerates)")]
    [SerializeField] private float maxHull = 30f;
    [SerializeField] private float hullRegenDelay = 3f;
    [SerializeField] private float hullRegenPerSecond = 5f;

    [Header("Corruption")]
    [SerializeField, Range(0f, 1f)] private float maxCorruptionFraction = 0.333f;
    [SerializeField] private float corruptionCurve = 2f;

    [Header("Life (does not recover)")]
    [SerializeField] private float maxLife = 100f;

    [Header("Damage")]
    [SerializeField] private float damagePerHit = 10f;
    [SerializeField] private float invulnerableTime = 0.5f;

    public float Hull { get; private set; }
    public float Life { get; private set; }
    public float Hull01 => Hull / maxHull;
    public float Life01 => Life / maxLife;
    public bool IsDead { get; private set; }

    private float lastHitTime = -999f;
    private float invulnerableUntil;

    public float CorruptionPoints =>
        GameFlow.Instance != null && GameFlow.Instance.Current == Phase.Challenge
            ? maxLife * maxCorruptionFraction * Mathf.Pow(GameFlow.Instance.ChallengeProgress01, corruptionCurve)
            : 0f;
    public float Corruption01 => maxLife > 0f ? CorruptionPoints / maxLife : 0f;

    private void OnEnable() => GameFlow.ChallengeEnded += OnChallengeEnded;
    private void OnDisable() => GameFlow.ChallengeEnded -= OnChallengeEnded;

    private void OnChallengeEnded(ChallengeResult result)
    {
        if (result == ChallengeResult.Won) return;
        LoseLife(CorruptionPoints);
    }

    private void Awake()
    {
        Hull = maxHull;
        Life = maxLife;
    }

    private void Update()
    {
        if (IsDead) return;

        if (Hull < maxHull && Time.time - lastHitTime >= hullRegenDelay)
            Hull = Mathf.Min(maxHull, Hull + hullRegenPerSecond * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsDead || !collision.CompareTag("EnemyBullet")) return;
        if (Time.time < invulnerableUntil) return;

        TakeDamage(damagePerHit);
        invulnerableUntil = Time.time + invulnerableTime;
    }

    private void TakeDamage(float amount)
    {
        lastHitTime = Time.time;
        float absorbed = Mathf.Min(Hull, amount);
        Hull -= absorbed;
        LoseLife(amount - absorbed);
    }

    private void LoseLife(float amount)
    {
        if (amount <= 0f) return;

        Life = Mathf.Max(0f, Life - amount);
        if (Life <= 0f && !IsDead)
        {
            IsDead = true;
            PlayerDied?.Invoke();
        }
    }
}