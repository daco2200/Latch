using TMPro;
using UnityEngine;

public class PhaseUI : MonoBehaviour
{
    [SerializeField] private GameFlow flow;
    [SerializeField] private Camera cam;                 // Clear Flags = Solid Color
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private TMP_Text timerText;

    [SerializeField] private string stableLabel = "STABLE";
    [SerializeField] private Color stableColor = new Color(0.10f, 0.10f, 0.10f);
    [SerializeField] private Color challengeColor = new Color(0.24f, 0.20f, 0.07f);
    [SerializeField] private Color textColor = Color.white;

    private void Update()
    {
        switch (flow.Current)
        {
            case Phase.Stable:
                bannerText.text = stableLabel;
                timerText.text = "";
                cam.backgroundColor = stableColor;
                break;

            case Phase.Challenge:
                bannerText.text = $"{flow.ChallengeTitle}  {flow.ChallengeStatus}";
                timerText.text = Mathf.CeilToInt(flow.TimeLeft).ToString();
                cam.backgroundColor = challengeColor;
                break;

            default:   // Title, Victory, GameOver: the screens handle the text
                bannerText.text = "";
                timerText.text = "";
                break;
        }

        bannerText.color = textColor;
        timerText.color = textColor;
    }
}