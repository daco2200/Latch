using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;

    [Header("Strafe (random per enemy)")]
    [SerializeField, Range(0f, 1f)] private float strafeChance = 0.4f;
    [SerializeField] private Vector2 strafeSpeedRange = new Vector2(0.8f, 2f);      // rad/s
    [SerializeField] private Vector2 strafeAmplitudeRange = new Vector2(0.3f, 1f);

    public bool Arrived { get; private set; }

    private Rigidbody2D rb2d;
    private float stopY, baseX, amplitude, frequency, strafeTime;
    private bool strafes;

    private void Awake() => rb2d = GetComponent<Rigidbody2D>();

    public void Init(float stopY, float maxAmplitude)
    {
        this.stopY = stopY;
        baseX = transform.position.x;
        strafes = maxAmplitude > 0.05f && Random.value < strafeChance;
        amplitude = Mathf.Min(Random.Range(strafeAmplitudeRange.x, strafeAmplitudeRange.y), maxAmplitude);
        frequency = Random.Range(strafeSpeedRange.x, strafeSpeedRange.y);
    }

    private void FixedUpdate()
    {
        Vector2 pos = rb2d.position;

        if (!Arrived)
        {
            pos.y = Mathf.MoveTowards(pos.y, stopY, speed * Time.fixedDeltaTime);
            Arrived = Mathf.Approximately(pos.y, stopY);
        }
        else if (strafes)
        {
            strafeTime += Time.fixedDeltaTime;
            pos.x = baseX + amplitude * Mathf.Sin(strafeTime * frequency);
        }

        rb2d.MovePosition(pos);
    }
}