using TMPro;
using UnityEngine;

public class PhaseUI : MonoBehaviour
{
    [SerializeField] private GameFlow flow;
    [SerializeField] private Camera cam;                 // Clear Flags = Solid Color
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private TMP_Text timerText;

    [SerializeField] private string calmLabel = "STABLE";
    [SerializeField] private Color calmColor = new Color(0.10f, 0.10f, 0.10f);
    [SerializeField] private Color challengeColor = new Color(0.24f, 0.20f, 0.07f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color overtimeColor = Color.red;

    private void Update()
    {
        switch (flow.Current)
        {
            case Phase.Calm:
                bannerText.text = calmLabel;
                bannerText.color = textColor;
                timerText.text = "";
                cam.backgroundColor = calmColor;
                break;

            case Phase.Challenge:
                bool over = flow.IsOvertime;
                bannerText.text = $"{flow.ChallengeTitle}  {flow.ChallengeStatus}";
                bannerText.color = over ? overtimeColor : textColor;
                timerText.text = over
                    ? $"+{Mathf.FloorToInt(flow.OvertimeSeconds)}"
                    : Mathf.CeilToInt(flow.TimeLeft).ToString();
                timerText.color = over ? overtimeColor : textColor;
                cam.backgroundColor = challengeColor;
                break;

            default:
                bannerText.text = "";
                timerText.text = "";
                break;
        }
    }
}