using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public enum Phase { Title, Stable, Challenge, Victory, GameOver }
public enum ChallengeResult { Won, TimedOut, Pulled }

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }
    private static bool skipTitle;

    [Header("Stable phase (seconds)")]
    [SerializeField] private Vector2 stableDurationRange = new Vector2(30f, 45f);

    [Header("Challenges: one entry per win needed")]
    [SerializeField] private int[] killTargets = { 8, 12, 16 };
    [SerializeField] private float challengeTime = 45f;

    [Header("Escalation")]
    [SerializeField, Range(0.2f, 1f)] private float minIntervalScale = 0.5f;
    [SerializeField] private float stableIntervalScale = 2f;   // >1 = slower spawning and fire during STABLE


    public Phase Current { get; private set; } = Phase.Title;
    public float TimeLeft { get; private set; }
    public int Wins { get; private set; }

    public float ChallengeProgress01 => Current == Phase.Challenge
        ? 1f - Mathf.Clamp01(TimeLeft / challengeTime)
        : 0f;

    public string ChallengeTitle => challenge != null ? challenge.Title : "";
    public string ChallengeStatus => challenge != null ? challenge.Status : "";

    public static float IntervalScale
    {
        get
        {
            if (Instance == null) return 1f;
            if (Instance.Current == Phase.Stable) return Instance.stableIntervalScale;
            return Mathf.Lerp(1f, Instance.minIntervalScale, Instance.ChallengeProgress01);
        }
    }

    public static event Action<ChallengeResult> ChallengeEnded;
    public event Action<Phase> PhaseChanged;

    private IChallenge challenge;
    private float stableTimer;

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
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) Pull();
#endif
        switch (Current)
        {
            case Phase.Stable:
                stableTimer -= Time.deltaTime;
                if (stableTimer <= 0f) StartChallenge();
                break;

            case Phase.Challenge:
                TimeLeft = Mathf.Max(0f, TimeLeft - Time.deltaTime);
                if (TimeLeft <= 0f) EndChallenge(ChallengeResult.TimedOut);
                break;
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        EnterStable();
    }

    public void Restart()
    {
        skipTitle = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pull()
    {
        if (Current == Phase.Challenge) EndChallenge(ChallengeResult.Pulled);
    }

    private IChallenge CreateChallenge(int index) => new KillChallenge(killTargets[index]);

    private void StartChallenge()
    {
        challenge = CreateChallenge(Wins);
        challenge.Begin();
        TimeLeft = challengeTime;
        SetPhase(Phase.Challenge);
    }

    private void OnEnemyDied(EnemyLife enemy)
    {
        if (Current != Phase.Challenge) return;

        challenge.OnEnemyKilled();
        if (challenge.IsComplete) EndChallenge(ChallengeResult.Won);
    }

    private void EndChallenge(ChallengeResult result)
    {
        if (result == ChallengeResult.Won) Wins++;

        ChallengeEnded?.Invoke(result);
        if (Current == Phase.GameOver) return;

        if (result == ChallengeResult.Won && Wins >= killTargets.Length)
        {
            SetPhase(Phase.Victory);
            Time.timeScale = 0f;
        }
        else EnterStable();
    }

    private void GameOver()
    {
        if (Current == Phase.GameOver || Current == Phase.Victory) return;
        SetPhase(Phase.GameOver);
        Time.timeScale = 0f;
    }

    private void EnterStable()
    {
        challenge = null;
        stableTimer = UnityEngine.Random.Range(stableDurationRange.x, stableDurationRange.y);
        SetPhase(Phase.Stable);
    }

    private void SetPhase(Phase phase)
    {
        Current = phase;
        PhaseChanged?.Invoke(phase);
    }
}