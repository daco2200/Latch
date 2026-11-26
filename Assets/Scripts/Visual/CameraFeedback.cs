using UnityEngine;

public class CameraFeedback : MonoBehaviour
{
    [Header("Shake")]
    [SerializeField] private float shootShake = 0.08f;
    [SerializeField] private float hitShake = 0.6f;
    [SerializeField] private float shakeDecay = 6f;
    [SerializeField] private float maxOffset = 0.35f;

    [Header("Kill zoom")]
    [SerializeField] private float killZoom = 0.2f;        // how much the ortho size shrinks
    [SerializeField] private float zoomRecover = 8f;

    private Camera cam;
    private Vector3 basePos;
    private float baseSize, trauma, zoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        basePos = transform.position;
        baseSize = cam.orthographicSize;
    }

    private void OnEnable()
    {
        PlayerShoot.Fired += OnFired;
        PlayerHealth.Damaged += OnDamaged;
        EnemyLife.AnyDied += OnKill;      // not raised by the shockwave, so only real kills
    }

    private void OnDisable()
    {
        PlayerShoot.Fired -= OnFired;
        PlayerHealth.Damaged -= OnDamaged;
        EnemyLife.AnyDied -= OnKill;
    }

    private void OnFired() => AddTrauma(shootShake);
    private void OnDamaged() => AddTrauma(hitShake);
    private void OnKill(EnemyLife enemy) => zoom = killZoom;

    private void AddTrauma(float v) => trauma = Mathf.Min(1f, trauma + v);

    private void LateUpdate()
    {
        trauma = Mathf.Max(0f, trauma - shakeDecay * Time.deltaTime);
        zoom = Mathf.Lerp(zoom, 0f, zoomRecover * Time.deltaTime);

        Vector2 offset = Random.insideUnitCircle * trauma * maxOffset;
        transform.position = basePos + (Vector3)offset;
        cam.orthographicSize = baseSize - zoom;
    }

    // World position of a viewport point, ignoring shake and zoom
    public Vector3 ViewportToWorldStable(Vector2 v)
    {
        float h = baseSize;
        float w = baseSize * cam.aspect;
        return new Vector3(basePos.x + (v.x - 0.5f) * 2f * w, basePos.y + (v.y - 0.5f) * 2f * h, 0f);
    }
}