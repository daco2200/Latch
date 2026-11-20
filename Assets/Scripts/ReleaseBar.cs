using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReleaseBar : MonoBehaviour
{
    [SerializeField] private float capacity = 100f;
    [SerializeField] private float gainPerHit = 5f;     // 20 hits to fill
    [SerializeField] private Image fill;                // optional: Image Type = Filled, Horizontal

    public float Value { get; private set; }
    public bool IsFull => Value >= capacity;
    public float Fill01 => capacity > 0f ? Mathf.Clamp01(Value / capacity) : 0f;

    private InputAction pullAction;

    private void Awake()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        pullAction = input != null ? input.actions.FindAction("Pull", false) : null;
        if (pullAction == null)
            Debug.LogError("ReleaseBar: action 'Pull' not found. Is this on the same object as PlayerInput?", this);
    }

    private void OnEnable()
    {
        EnemyLife.AnyHit += OnEnemyHit;
        GameFlow.ChallengeEnded += OnChallengeEnded;
    }

    private void OnDisable()
    {
        EnemyLife.AnyHit -= OnEnemyHit;
        GameFlow.ChallengeEnded -= OnChallengeEnded;
    }

    private void OnEnemyHit(EnemyLife enemy)
    {
        // Only fills during a challenge
        if (GameFlow.Instance == null || GameFlow.Instance.Current != Phase.Challenge) return;
        Value = Mathf.Min(capacity, Value + gainPerHit);
    }

    private void OnChallengeEnded(ChallengeResult result) => Value = 0f;

    private void Update()
    {
        if (fill != null) fill.fillAmount = Fill01;

        if (pullAction != null && pullAction.WasPressedThisFrame())
        {
            Debug.Log($"Pull pressed. Full: {IsFull} ({Value}/{capacity})");
            if (IsFull)
            {
                GameFlow.Instance.Pull();
                Debug.Log("Pull executed");
            }
        }
    }
}