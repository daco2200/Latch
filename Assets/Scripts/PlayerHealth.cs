using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int hull = 3;
    [SerializeField] private float invulnerableTime = 1f;

    private int maxHull;
    private float invulnerableUntil;

    private void Awake() => maxHull = hull;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("EnemyBullet")) return;
        if (hull <= 0 || Time.time < invulnerableUntil) return;

        hull--;
        invulnerableUntil = Time.time + invulnerableTime;
        Debug.Log($"Hull: {hull}/{maxHull}");
        // At 0 nothing else happens yet: no death for now
    }
}
