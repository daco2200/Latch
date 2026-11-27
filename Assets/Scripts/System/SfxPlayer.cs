using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [Range(0f, 1f)] public float master = 0.8f;

    [SerializeField] private Sfx shoot = new Sfx();
    [SerializeField] private Sfx hit = new Sfx();
    [SerializeField] private Sfx kill = new Sfx();
    [SerializeField] private Sfx dissolve = new Sfx();       // enemies removed by the shockwave
    [SerializeField] private Sfx playerHit = new Sfx();
    [SerializeField] private Sfx challengeStart = new Sfx();
    [SerializeField] private Sfx pull = new Sfx();
    [SerializeField] private Sfx releaseReady = new Sfx();
    [SerializeField] private Sfx victory = new Sfx();
    [SerializeField] private Sfx gameOver = new Sfx();

    private GameFlow flow;

    private void Awake()
    {
        shoot.Init(gameObject); hit.Init(gameObject); kill.Init(gameObject);
        dissolve.Init(gameObject); playerHit.Init(gameObject);
        challengeStart.Init(gameObject); pull.Init(gameObject);
        releaseReady.Init(gameObject); victory.Init(gameObject); gameOver.Init(gameObject);
    }

    private void OnEnable()
    {
        PlayerShoot.Fired += OnFired;
        PlayerHealth.Damaged += OnDamaged;
        EnemyLife.AnyHit += OnHit;
        EnemyLife.AnyDied += OnKill;
        EnemyLife.AnyDissolved += OnDissolve;
        ReleaseBar.Ready += OnReady;
        GameFlow.ChallengeStarted += OnChallengeStarted;
        GameFlow.ChallengeEnded += OnChallengeEnded;
    }

    private void OnDisable()
    {
        PlayerShoot.Fired -= OnFired;
        PlayerHealth.Damaged -= OnDamaged;
        EnemyLife.AnyHit -= OnHit;
        EnemyLife.AnyDied -= OnKill;
        EnemyLife.AnyDissolved -= OnDissolve;
        ReleaseBar.Ready -= OnReady;
        GameFlow.ChallengeStarted -= OnChallengeStarted;
        GameFlow.ChallengeEnded -= OnChallengeEnded;
    }

    private void Start()
    {
        flow = GameFlow.Instance;
        if (flow != null) flow.PhaseChanged += OnPhase;
    }

    private void OnDestroy()
    {
        if (flow != null) flow.PhaseChanged -= OnPhase;
    }

    private void OnFired() => shoot.Play(master);
    private void OnHit(EnemyLife e) => hit.Play(master);
    private void OnKill(EnemyLife e) => kill.Play(master);
    private void OnDissolve(EnemyLife e) => dissolve.Play(master);
    private void OnDamaged() => playerHit.Play(master);
    private void OnReady() => releaseReady.Play(master);
    private void OnChallengeStarted(IChallenge c) => challengeStart.Play(master);
    private void OnChallengeEnded(ChallengeResult r) { if (r == ChallengeResult.Pulled) pull.Play(master); }

    private void OnPhase(Phase phase)
    {
        if (phase == Phase.Victory) victory.Play(master);
        else if (phase == Phase.GameOver) gameOver.Play(master);
    }
}