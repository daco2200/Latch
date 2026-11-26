using UnityEngine;

public class Fragment : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f;

    private SpriteRenderer sr;
    private Vector2 velocity;
    private float spin, t;

    private void Awake() => sr = GetComponent<SpriteRenderer>();

    public void Launch(Vector2 v, float life = -1f)
    {
        velocity = v;
        spin = Random.Range(-360f, 360f);
        if (life > 0f) lifetime = life;
    }

    private void Update()
    {
        t += Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, spin * Time.deltaTime);

        Color c = sr.color;
        c.a = 1f - t / lifetime;
        sr.color = c;

        if (t >= lifetime) Destroy(gameObject);
    }
}