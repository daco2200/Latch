
using System.Collections;
using UnityEngine;

public class PlayerVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] bars = new Transform[5];
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Idle")]
    [SerializeField] private float idleAmount = 0.03f;
    [SerializeField] private float idleSpeed = 2f;

    [Header("Movement")]
    [SerializeField] private float movementAmount = 0.2f;

    [Header("Shooting")]
    [SerializeField] private float shootAmount = 0.3f;
    [SerializeField] private float shootDuration = 0.12f;

    [Header("Damage")]
    [SerializeField] private float hitAmount = 0.5f;
    [SerializeField] private float hitDuration = 0.2f;

    [Header("Damage Flash")]
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private float flashSpeed = 12f;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 12f;

    [Header("Death")]
    [SerializeField] private float deathDuration = 0.8f;

    private Vector3[] originalScales;
    private float[] currentScales;
    private float[] velocities;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    private float shootTimer;
    private float hitTimer;
    private float flashAmount;

    private bool isDead;

    private void Awake()
    {
        int count = bars.Length;

        originalScales = new Vector3[count];
        currentScales = new float[count];
        velocities = new float[count];

        spriteRenderers = new SpriteRenderer[count];
        originalColors = new Color[count];

        for (int i = 0; i < count; i++)
        {
            if (bars[i] == null) continue;

            originalScales[i] = bars[i].localScale;
            currentScales[i] = 1f;

            spriteRenderers[i] =
                bars[i].GetComponent<SpriteRenderer>();

            if (spriteRenderers[i] != null)
                originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void OnEnable()
    {
        PlayerHealth.Damaged += TriggerHit;
        PlayerShoot.Fired += TriggerShoot;
    }

    private void OnDisable()
    {
        PlayerHealth.Damaged -= TriggerHit;
        PlayerShoot.Fired -= TriggerShoot;
    }

    private void Update()
    {
        if (isDead) return;

        if (playerHealth.IsDead)
        {
            TriggerDeath();
            return;
        }

        AnimateBars();
        AnimateFlash();

        shootTimer = Mathf.Max(0f, shootTimer - Time.deltaTime);
        hitTimer = Mathf.Max(0f, hitTimer - Time.deltaTime);
    }

    private void AnimateBars()
    {
        float time = Time.time;
        float movement = playerMovement.MoveInput;

        float shootPulse = shootTimer > 0f
            ? shootTimer / shootDuration : 0f;

        float hitPulse = hitTimer > 0f
            ? hitTimer / hitDuration : 0f;

        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null) continue;

            float target = 1f;

            // Central bar: subtle idle movement
            if (i == 2)
                target += Mathf.Sin(time * idleSpeed)
                    * idleAmount * 0.15f;

            // Horizontal movement
            if (i == 1)
                target -= movement * movementAmount;

            if (i == 3)
                target += movement * movementAmount;

            // Shooting: bars 2 and 4
            if (i == 1 || i == 3)
                target -= shootAmount * shootPulse;

            // Damage deformation
            if (hitPulse > 0f)
                target += hitAmount * hitPulse;

            currentScales[i] = Mathf.SmoothDamp(
                currentScales[i],
                target,
                ref velocities[i],
                1f / smoothSpeed
            );

            Vector3 scale = originalScales[i];
            scale.y *= currentScales[i];

            bars[i].localScale = scale;
        }
    }

    private void AnimateFlash()
    {
        flashAmount = Mathf.MoveTowards(
            flashAmount,
            0f,
            flashSpeed * Time.deltaTime
        );

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            spriteRenderers[i].color = Color.Lerp(
                originalColors[i],
                hitFlashColor,
                flashAmount
            );
        }
    }

    public void TriggerShoot()
    {
        if (isDead) return;

        shootTimer = shootDuration;
    }

    public void TriggerHit()
    {
        if (isDead) return;

        hitTimer = hitDuration;
        flashAmount = 1f;
    }

    public void TriggerDeath()
    {
        if (isDead) return;

        isDead = true;
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        float elapsed = 0f;

        Vector3[] startScales = new Vector3[bars.Length];

        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] != null)
                startScales[i] = bars[i].localScale;
        }

        while (elapsed < deathDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < bars.Length; i++)
            {
                if (bars[i] == null) continue;

                float delay = i * 0.08f;
                float duration = Mathf.Max(0.01f, deathDuration - delay);

                float progress = Mathf.Clamp01(
                    (elapsed - delay) / duration
                );

                Vector3 scale = startScales[i];
                scale.y *= 1f - progress;

                bars[i].localScale = scale;
            }

            yield return null;
        }

        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null) continue;

            bars[i].localScale = new Vector3(
                originalScales[i].x,
                0f,
                originalScales[i].z
            );
        }
    }
}
