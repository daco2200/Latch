using UnityEngine;

public class MorphVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;     // on a child object
    [SerializeField] private EnemyLife life;

    [Header("Deformation")]
    [SerializeField] private float masterAmplitude = 0.08f;   // wobble while in a challenge
    [SerializeField] private float damageAmplitude = 0.25f;   // extra wobble as health drops
    [SerializeField] private float speed = 6f;
    [SerializeField] private float rotationWobble = 8f;       // degrees

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color telegraphColor = new Color(0.85f, 0.29f, 0.29f);
    [SerializeField] private float hitFlashTime = 0.06f;

    private Vector3 baseScale;
    private float seed, telegraphUntil, flashUntil;

    private void Awake()
    {
        if (life == null) life = GetComponent<EnemyLife>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        baseScale = sprite.transform.localScale;
        seed = Random.value * 100f;
    }

    private void OnEnable() { life.Hit += OnHit; }
    private void OnDisable() { life.Hit -= OnHit; }

    private void OnHit(EnemyLife enemy) => flashUntil = Time.time + hitFlashTime;

    public void Telegraph(float duration) => telegraphUntil = Time.time + duration;

    private void Update()
    {
        bool telegraphing = Time.time < telegraphUntil;
        bool flashing = Time.time < flashUntil;

        sprite.color = flashing ? Color.white : telegraphing ? telegraphColor : normalColor;

        if (telegraphing)   // stops trembling right before shooting
        {
            sprite.transform.localScale = baseScale;
            sprite.transform.localRotation = Quaternion.identity;
            return;
        }

        bool master = GameFlow.Instance != null && GameFlow.Instance.Current == Phase.Challenge;
        float damage = 1f - life.Health01;
        float amp = (master ? masterAmplitude : 0f) + damageAmplitude * damage;

        float t = Time.time * speed + seed;
        float sx = 1f + (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f * amp;
        float sy = 1f + (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f * amp;
        sprite.transform.localScale = new Vector3(baseScale.x * sx, baseScale.y * sy, 1f);
        sprite.transform.localRotation = Quaternion.Euler(0f, 0f,
            (Mathf.PerlinNoise(t, t) - 0.5f) * 2f * rotationWobble * (0.3f + damage));
    }
}