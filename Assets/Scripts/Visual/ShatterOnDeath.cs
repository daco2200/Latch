using UnityEngine;

public class ShatterOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject fragmentPrefab;   // small white square with a Fragment script
    [SerializeField] private int count = 8;
    [SerializeField] private float speed = 6f;

    private EnemyLife life;

    private void Awake() => life = GetComponent<EnemyLife>();
    private void OnEnable() { life.Died += OnDied; }
    private void OnDisable() { life.Died -= OnDied; }

    private void OnDied(EnemyLife enemy)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject f = Instantiate(fragmentPrefab, transform.position,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            float a = (360f / count * i + Random.Range(-15f, 15f)) * Mathf.Deg2Rad;
            f.GetComponent<Fragment>().Launch(
                new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * speed * Random.Range(0.6f, 1.2f));
        }
    }
}