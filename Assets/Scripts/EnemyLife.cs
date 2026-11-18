
using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] private int life = 3;
    [SerializeField] private float cooldown = 2f;

    private int maxLife;
    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;

    public event System.Action<EnemyLife> Died;
    [SerializeField] private bool respawn;

    private void Awake()
    {
        maxLife = life;
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet")) return;
        if (life <= 0) return;

        life--;

        if (life <= 0)
        {
            Died?.Invoke(this);
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
