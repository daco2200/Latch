public interface IChallenge
{
    string Title { get; }
    string Status { get; }
    bool IsComplete { get; }
    void Begin();
    void OnEnemyKilled();
}

public class KillChallenge : IChallenge
{
    private readonly int required;
    private int kills;

    public KillChallenge(int required) { this.required = required; }

    public string Title => "KILL";
    public string Status => $"{kills}/{required}";
    public bool IsComplete => kills >= required;

    public void Begin() => kills = 0;
    public void OnEnemyKilled() => kills++;
}