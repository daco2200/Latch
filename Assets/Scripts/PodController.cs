using UnityEngine;

public class PodController : MonoBehaviour
{
    [SerializeField] private Transform pointer;            // the aim point

    [Header("Position (local to the ship)")]
    [SerializeField] private float sideOffset = 1.6f;
    [SerializeField] private float height = 0.4f;
    [SerializeField] private float switchMargin = 0.8f;

    [Header("Travel between sides")]
    [SerializeField] private float travelTime = 0.2f;
    [SerializeField] private float arcHeight = 0.6f;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 720f;     // degrees per second, 0 = instant

    [Header("Recoil")]
    [SerializeField] private float kickDistance = 0.12f;   // push back per shot
    [SerializeField] private float kickRecover = 2f;       // units per second back to rest
    [SerializeField] private float maxKick = 0.25f;

    private int side = 1;                                  // +1 right, -1 left
    private Vector3 current, from, to;                     // position without recoil
    private float t = 1f, kick;

    private void Awake()
    {
        current = new Vector3(side * sideOffset, height, 0f);
        transform.localPosition = current;
    }

    private void OnEnable() => PlayerShoot.Fired += OnFired;
    private void OnDisable() => PlayerShoot.Fired -= OnFired;

    private void OnFired() => kick = Mathf.Min(maxKick, kick + kickDistance);

    private void Update()
    {
        float dx = pointer.position.x - transform.parent.position.x;
        if (side > 0 && dx < -switchMargin) StartSwitch(-1);
        else if (side < 0 && dx > switchMargin) StartSwitch(1);

        if (t < 1f)
        {
            t = Mathf.Min(1f, t + Time.deltaTime / travelTime);
            float e = Mathf.SmoothStep(0f, 1f, t);
            current = Vector3.Lerp(from, to, e);
            current.y += Mathf.Sin(e * Mathf.PI) * arcHeight;
        }

        Vector2 dir = pointer.position - transform.position;
        float target = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;   // nub points up at rest
        float angle = rotateSpeed <= 0f
            ? target
            : Mathf.MoveTowardsAngle(transform.eulerAngles.z, target, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Recoil: pushed back along the aim direction, then eases to rest
        kick = Mathf.MoveTowards(kick, 0f, kickRecover * Time.deltaTime);
        Vector3 kickLocal = transform.parent.InverseTransformDirection(-transform.up) * kick;
        transform.localPosition = current + kickLocal;
    }

    private void StartSwitch(int newSide)
    {
        side = newSide;
        from = current;
        to = new Vector3(side * sideOffset, height, 0f);
        t = 0f;
    }
}