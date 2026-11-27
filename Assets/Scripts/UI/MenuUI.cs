
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject creditsPanel;

    private void OnEnable()
    {
        if (GameFlow.Instance != null)
            GameFlow.Instance.PhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (GameFlow.Instance != null)
            GameFlow.Instance.PhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(Phase phase)
    {
        HideAll();

        switch (phase)
        {
            case Phase.Title:
                startPanel.SetActive(true);
                break;

            case Phase.GameOver:
            case Phase.Victory:
                restartPanel.SetActive(true);
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
        GameFlow.Instance.StartGame();
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
        GameFlow.Instance.Restart();
    }
}
