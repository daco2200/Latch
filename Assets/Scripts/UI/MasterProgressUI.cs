using UnityEngine;
using UnityEngine.UI;

public class MasterProgressUI : MonoBehaviour
{
    [SerializeField] private GameFlow flow;
    [SerializeField] private Image[] markers;   // 3 circles + 1 skull, in order
    [SerializeField] private Color pending = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color done = Color.white;

    private void Update()
    {
        for (int i = 0; i < markers.Length; i++)
            markers[i].color = i < flow.Wins ? done : pending;
    }
}