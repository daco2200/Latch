public interface IChallenge
{
    string Title { get; }
    string Status { get; }
    bool IsComplete { get; }
    bool IsBoss { get; }
    bool UsesMarks { get; }
    void Begin();
    void OnEnemyKilled(EnemyLife enemy);
}

public class KillChallenge : IChallenge
{
    private readonly int required;
    private int kills;

    public KillChallenge(int required) { this.required = required; }

    public string Title => "KILL";
    public string Status => $"{kills}/{required}";
    public bool IsComplete => kills >= required;
    public bool IsBoss => false;
    public bool UsesMarks => false;

    public void Begin() => kills = 0;
    public void OnEnemyKilled(EnemyLife enemy) => kills++;
}

public class MarkedKillChallenge : IChallenge
{
    private readonly int required;
    private int kills;

    public MarkedKillChallenge(int required) { this.required = required; }

    public string Title => "MARKED";
    public string Status => $"{kills}/{required}";
    public bool IsComplete => kills >= required;
    public bool IsBoss => false;
    public bool UsesMarks => true;

    public void Begin() => kills = 0;

    public void OnEnemyKilled(EnemyLife enemy)
    {
        EnemyMark mark = enemy.GetComponent<EnemyMark>();
        if (mark != null && mark.IsMarked) kills++;   // unmarked kills don't count
    }
}

public class BossChallenge : IChallenge
{
    private bool defeated;

    public string Title => "BOSS";
    public string Status => defeated ? "DOWN" : "";
    public bool IsComplete => defeated;
    public bool IsBoss => true;
    public bool UsesMarks => false;

    public void Begin() => defeated = false;
    public void OnEnemyKilled(EnemyLife enemy) { if (enemy.IsBoss) defeated = true; }
}