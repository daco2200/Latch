// AlterationManager.cs (one object in the scene)
using UnityEngine;

public enum AlterationType { InvertControls, WeaponJam }

public class AlterationManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float noneChance = 0f;   // chance of no alteration

    [Header("Weapon jam windows")]
    [SerializeField] private float jamPeriod = 6f;       // length of a full cycle
    [SerializeField] private float jamDuration = 2f;     // jammed part, at the end of each cycle

    public string Label { get; private set; } = "";

    private AlterationType? current;
    private float cycleTimer;

    private void OnEnable()
    {
        GameFlow.ChallengeStarted += OnStarted;
        GameFlow.ChallengeEnded += OnEnded;
        RuleState.Clear();
    }

    private void OnDisable()
    {
        GameFlow.ChallengeStarted -= OnStarted;
        GameFlow.ChallengeEnded -= OnEnded;
        RuleState.Clear();
    }

    private void OnStarted(IChallenge challenge)
    {
        RuleState.Clear();
        cycleTimer = 0f;

        if (Random.value < noneChance)
        {
            current = null;
            Label = "";
            return;
        }

        var values = (AlterationType[])System.Enum.GetValues(typeof(AlterationType));
        AlterationType type = values[Random.Range(0, values.Length)];
        current = type;

        if (type == AlterationType.InvertControls)
        {
            RuleState.InvertControls = true;
            Label = "INVERTED";
        }
        else Label = "JAM";
    }

    private void OnEnded(ChallengeResult result)
    {
        RuleState.Clear();
        current = null;
        Label = "";
    }

    private void Update()
    {
        if (current != AlterationType.WeaponJam) return;

        cycleTimer += Time.deltaTime;
        RuleState.WeaponJammed = cycleTimer % jamPeriod >= jamPeriod - jamDuration;
        Label = RuleState.WeaponJammed ? "JAMMED" : "JAM";
    }
}