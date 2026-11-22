using UnityEngine;

public class EnemyMark : MonoBehaviour
{
    [SerializeField] private GameObject visual;   // ring/outline child, disabled by default

    public static int AliveMarked { get; private set; }
    public static void ResetCount() => AliveMarked = 0;

    public bool IsMarked { get; private set; }

    public void SetMarked(bool value)
    {
        if (value == IsMarked) return;
        IsMarked = value;
        AliveMarked += value ? 1 : -1;
        if (visual != null) visual.SetActive(value);
    }

    private void OnDestroy()
    {
        if (IsMarked) AliveMarked--;
    }
}