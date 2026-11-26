using UnityEngine;

public class HitSparks : MonoBehaviour
{
    [SerializeField] private GameObject fragmentPrefab;   // the same small square as the death shatter
    [SerializeField] private int count = 3;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float lifetime = 0.25f;
    [SerializeField] private float cone = 140f;           // spread around straight down, toward the player

    private EnemyLife life;

    private void Awake() => life = GetComponent<EnemyLife>();
    private void OnEnable() { life.Hit += OnHit; }
    private void OnDisable() { life.Hit -= OnHit; }

    private void OnHit(EnemyLife enemy)
    {
        for (int i = 0; i < count; i++)
        {
            float a = Random.Range(-cone * 0.5f, cone * 0.5f);
            Vector2 dir = Quaternion.Euler(0f, 0f, a) * Vector2.down;
            GameObject f = Instantiate(fragmentPrefab, transform.position,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            f.GetComponent<Fragment>().Launch(dir * speed * Random.Range(0.6f, 1.2f), lifetime);
        }
    }
}