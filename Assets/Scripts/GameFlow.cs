using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public enum Phase { Title, Calm, Challenge, Victory, GameOver }
public enum FailReason { Pull }

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }
    private static bool skipTitle;   // set by Restart so the game starts directly

    [Header("Calm phase (seconds)")]
    [SerializeField] private Vector2 calmDurationRange = new Vector2(90f, 120f);

    [Header("Challenges: one entry per win needed")]
    [SerializeField] private int[] killTargets = { 8, 12, 16 };
    [SerializeField] private float challengeTime = 45f;

    [Header("Escalation")]
    [SerializeField, Range(0.2f, 1f)] private float minIntervalScale = 0.5f;

    [Header("Overtime drain (life per second)")]
    [SerializeField] private float baseDrain = 1f;
    [SerializeField] private float rampInterval = 10f;
    [SerializeField] private float rampStep = 1f;

    public Phase Current { get; private set; } = Phase.Title;
    public float TimeLeft { get; private set; }
    public float OvertimeSeconds { get; private set; }
    public int Wins { get; private set; }

    public bool IsOvertime => Current == Phase.Challenge && TimeLeft <= 0f;

    public float DrainPerSecond => IsOvertime
        ? baseDrain + rampStep * Mathf.Floor(OvertimeSeconds / rampInterval)
        : 0f;

    public float Aggression01 => Current != Phase.Challenge ? 0f
        : 1f - Mathf.Clamp01(TimeLeft / challengeTime);

    public string ChallengeTitle => challenge != null ? challenge.Title : "";
    public string ChallengeStatus => challenge != null ? challenge.Status : "";

    public static float IntervalScale =>
        Instance != null ? Mathf.Lerp(1f, Instance.minIntervalScale, Instance.Aggression01) : 1f;

    public event Action<Phase> PhaseChanged;
    public event Action<FailReason> ChallengeFailed;
    public event Action ChallengeCompleted;

    private IChallenge challenge;
    private float calmTimer;

    private void Awake() => Instance = this;

    private void OnEnable()
    {
        EnemyLife.AnyDied += OnEnemyDied;
        PlayerHealth.PlayerDied += GameOver;
    }

    private void OnDisable()
    {
        EnemyLife.AnyDied -= OnEnemyDied;
        PlayerHealth.PlayerDied -= GameOver;
    }

    private void Start()
    {
        if (skipTitle) { skipTitle = false; StartGame(); }
        else { SetPhase(Phase.Title); Time.timeScale = 0f; }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) Pull();   // debug until the release bar exists
#endif
        switch (Current)
        {
            case Phase.Calm:
                calmTimer -= Time.deltaTime;
                if (calmTimer <= 0f) StartChallenge();
                break;

            case Phase.Challenge:
                if (TimeLeft > 0f) TimeLeft = Mathf.Max(0f, TimeLeft - Time.deltaTime);
                else OvertimeSeconds += Time.deltaTime;
                break;
        }
    }

    // Called by the Start button
    public void StartGame()
    {
        Time.timeScale = 1f;
        EnterCalm();
    }

    public void Restart()
    {
        skipTitle = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pull()
    {
        if (Current != Phase.Challenge) return;
        ChallengeFailed?.Invoke(FailReason.Pull);
        EnterCalm();
    }

    private IChallenge CreateChallenge(int index) => new KillChallenge(killTargets[index]);

    private void StartChallenge()
    {
        challenge = CreateChallenge(Wins);
        challenge.Begin();
        TimeLeft = challengeTime;
        OvertimeSeconds = 0f;
        SetPhase(Phase.Challenge);
    }

    private void OnEnemyDied(EnemyLife enemy)
    {
        if (Current != Phase.Challenge) return;

        challenge.OnEnemyKilled();
        if (challenge.IsComplete) Complete();
    }

    private void Complete()
    {
        Wins++;
        ChallengeCompleted?.Invoke();

        if (Wins >= killTargets.Length)
        {
            SetPhase(Phase.Victory);
            Time.timeScale = 0f;
        }
        else EnterCalm();
    }

    private void GameOver()
    {
        if (Current == Phase.GameOver || Current == Phase.Victory) return;
        SetPhase(Phase.GameOver);
        Time.timeScale = 0f;
    }

    private void EnterCalm()
    {
        challenge = null;
        calmTimer = UnityEngine.Random.Range(calmDurationRange.x, calmDurationRange.y);
        SetPhase(Phase.Calm);
    }

    private void SetPhase(Phase phase)
    {
        Current = phase;
        PhaseChanged?.Invoke(phase);
    }
}