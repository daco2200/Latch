using TMPro;
using UnityEngine;

public class ScreensUI : MonoBehaviour
{
    [SerializeField] private GameFlow flow;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endText;

    private void OnEnable()
    {
        flow.PhaseChanged += Refresh;
        Refresh(flow.Current);
    }

    private void OnDisable() => flow.PhaseChanged -= Refresh;

    private void Refresh(Phase phase)
    {
        titlePanel.SetActive(phase == Phase.Title);
        endPanel.SetActive(phase == Phase.Victory || phase == Phase.GameOver);
        endText.text = phase == Phase.Victory ? "VICTORY" : "GAME OVER";
    }
}