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

    [Header("Targets per level (index 0 = level 1)")]
    [SerializeField] private int[] killTargets = { 8, 12, 16 };
    [SerializeField] private int[] markedTargets = { 4, 8, 12 };

    [Header("Time limits (seconds)")]
    [SerializeField] private float[] levelTimes = { 35f, 45f, 60f };   // index 0 = level 1
    [SerializeField] private float bossTime = 70f;

    [Header("Escalation")]
    [SerializeField, Range(0.2f, 1f)] private float minIntervalScale = 0.5f;
    [SerializeField] private float stableIntervalScale = 2f;

    public Phase Current { get; private set; } = Phase.Title;
    public float TimeLeft { get; private set; }
    public int Wins { get; private set; }

    public int LevelCount => killTargets.Length;    // challenges before the boss
    public bool IsBossChallenge => challenge != null && challenge.IsBoss;
    public bool MarkingActive => Current == Phase.Challenge && challenge != null && challenge.UsesMarks;

    public float ChallengeProgress01 => Current == Phase.Challenge && timeLimit > 0f
        ? 1f - Mathf.Clamp01(TimeLeft / timeLimit)
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

    public static event Action<IChallenge> ChallengeStarted;
    public static event Action<ChallengeResult> ChallengeEnded;   // raised BEFORE leaving the challenge phase
    public event Action<Phase> PhaseChanged;

    private IChallenge challenge;
    private float stableTimer;
    private float timeLimit;

    private void Awake()
    {
        Instance = this;
        EnemyMark.ResetCount();
    }

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
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) Pull();   // debug: ignores the release bar
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

    public void Restart()   // the scene must be in Build Settings
    {
        skipTitle = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pull()
    {
        if (Current == Phase.Challenge) EndChallenge(ChallengeResult.Pulled);
    }

    // Add new objectives here (and a new case in the Random range)
    private IChallenge CreateChallenge()
    {
        if (Wins >= LevelCount) return new BossChallenge();

        int level = Wins;
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0: return new KillChallenge(killTargets[level]);
            default: return new MarkedKillChallenge(markedTargets[level]);
        }
    }

    private void StartChallenge()
    {
        challenge = CreateChallenge();
        challenge.Begin();
        timeLimit = challenge.IsBoss ? bossTime : levelTimes[Mathf.Min(Wins, levelTimes.Length - 1)];
        TimeLeft = timeLimit;

        SetPhase(Phase.Challenge);
        ChallengeStarted?.Invoke(challenge);
    }

    private void OnEnemyDied(EnemyLife enemy)
    {
        if (Current != Phase.Challenge) return;

        challenge.OnEnemyKilled(enemy);
        if (challenge.IsComplete) EndChallenge(ChallengeResult.Won);
    }

    private void EndChallenge(ChallengeResult result)
    {
        if (result == ChallengeResult.Won) Wins++;

        ChallengeEnded?.Invoke(result);          // PlayerHealth applies corruption here (it can kill)
        if (Current == Phase.GameOver) return;   // the player died to corruption

        if (result == ChallengeResult.Won && Wins > LevelCount)   // the boss was the last step
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