
using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    public static event System.Action<EnemyLife> AnyDied;
    public static event System.Action<EnemyLife> AnyHit;
    public static event System.Action<EnemyLife> AnyDissolved;

    public event System.Action<EnemyLife> Died;

    [SerializeField] private int life = 3;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private bool respawn;
    [SerializeField] private bool isBoss;
    public bool IsBoss => isBoss;

    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private int maxLife;

    public event System.Action<EnemyLife> Hit;
    public float Health01 => maxLife > 0 ? (float)life / maxLife : 0f;

    private void Awake()
    {
        maxLife = life;
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Dissolve()
    {
        if (life <= 0) return;
        life = 0;
        AnyDissolved?.Invoke(this);
        Died?.Invoke(this);   // frees its spawner slot; AnyDied is NOT raised, so it doesn't count as a kill
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet")) return;
        if (life <= 0) return;

        life--;

        // Notificar cada impacto
        AnyHit?.Invoke(this);
        Hit?.Invoke(this);

        if (life <= 0)
        {
            Died?.Invoke(this);
            AnyDied?.Invoke(this);

            if (respawn) StartCoroutine(Respawn());
            else Destroy(gameObject);
        }
    }

    private IEnumerator Respawn()
    {
        enemyCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(cooldown);

        life = maxLife;
        spriteRenderer.enabled = true;
        enemyCollider.enabled = true;
    }
}
