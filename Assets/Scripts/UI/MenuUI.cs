
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("End Screen")]
    [SerializeField] private TMP_Text endText;

    private GameFlow flow;

    private void Start()
    {
        flow = GameFlow.Instance;

        if (flow != null)
        {
            flow.PhaseChanged += OnPhaseChanged;
            OnPhaseChanged(flow.Current);
        }
        else
        {
            Debug.LogError("GameFlow.Instance no encontrado.");
        }
    }

    private void OnDestroy()
    {
        if (flow != null)
            flow.PhaseChanged -= OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (flow != null)
            flow.PhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(Phase phase)
    {
        HideAll();

        switch (phase)
        {
            case Phase.Title:
                startPanel.SetActive(true);
                break;

            case Phase.Stable:
            case Phase.Challenge:
                break;

            case Phase.GameOver:
            case Phase.Victory:
                restartPanel.SetActive(true);
                endText.text = phase == Phase.Victory ? "VICTORY" : "GAME OVER";
                break;
        }
    }

    private void HideAll()
    {
        startPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        restartPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void StartGame()
    {
        flow.StartGame();
    }

    public void ShowTutorial()
    {
        HideAll();
        tutorialPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        HideAll();
        creditsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        HideAll();
        startPanel.SetActive(true);
    }

    public void RestartGame()
    {
        flow.Restart();
    }
}
