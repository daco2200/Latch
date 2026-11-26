using UnityEngine;
using UnityEngine.UI;

public class MasterProgressUI : MonoBehaviour
{
    [SerializeField] private GameFlow flow;
    [SerializeField] private Image[] markers;          // 3 waves + 1 boss, in order
    [SerializeField] private Sprite[] pendingSprites;  // hexagon outline x3, skull
    [SerializeField] private Sprite[] doneSprites;     // hexagon with inner hexagon x3, skull
    [SerializeField] private Color pending = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color done = Color.white;

    private void Update()
    {
        for (int i = 0; i < markers.Length; i++)
        {
            bool isDone = i < flow.Wins;
            markers[i].sprite = isDone ? doneSprites[i] : pendingSprites[i];
            markers[i].color = isDone ? done : pending;
        }
    }
}