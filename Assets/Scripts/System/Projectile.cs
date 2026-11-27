
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeSpan;
    [SerializeField] private string hitTag = "Enemy";
    [SerializeField] private TrailRenderer trail;

    private Rigidbody2D rb2d;
    private Coroutine lifeCoroutine;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb2d.linearVelocity = transform.up * speed;
        if (trail != null) trail.Clear();
        lifeCoroutine = StartCoroutine(DeactivateBullet());
    }

    private void OnDisable()
    {
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        rb2d.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(hitTag))
            gameObject.SetActive(false);
    }

    private IEnumerator DeactivateBullet()
    {
        yield return new WaitForSeconds(lifeSpan);

        gameObject.SetActive(false);
    }
}
