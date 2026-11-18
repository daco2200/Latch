using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab and pool")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyBulletPool bulletPool;

    [Header("Rail (centered on this transform)")]
    [SerializeField] private float railHalfWidth = 8f;
    [SerializeField] private int slotCount = 6;
    [SerializeField] private float enemyHalfWidth = 0.4f;

    [Header("Stop rows (world Y)")]
    [SerializeField] private float[] rowY = { 5f, 3.5f, 2f };

    [Header("Spawn rules")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int minSlotDistance = 2;
    [SerializeField, Range(0f, 0.4f)] private float jitter = 0.1f;

    private EnemyLife[] occupants;
    private int[] slotRow;
    private readonly List<int> slotCandidates = new List<int>();
    private readonly List<int> rowCandidates = new List<int>();
    private float timer;
    private int lastSlot = -99;

    private float SlotWidth => 2f * railHalfWidth / slotCount;

    private void Awake()
    {
        occupants = new EnemyLife[slotCount];
        slotRow = new int[slotCount];
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        TrySpawn();
        timer = spawnInterval * GameFlow.IntervalScale;
    }

    private bool TrySpawn()
    {
        slotCandidates.Clear();
        for (int i = 0; i < slotCount; i++)
        {
            if (occupants[i] == null && Mathf.Abs(i - lastSlot) >= minSlotDistance)
                slotCandidates.Add(i);
        }
        if (slotCandidates.Count == 0) return false;

        int slot = slotCandidates[Random.Range(0, slotCandidates.Count)];
        int row = PickRow(slot);
        if (row < 0) return false;

        float slotW = SlotWidth;
        float center = transform.position.x - railHalfWidth + (slot + 0.5f) * slotW;
        float offset = Random.Range(-jitter, jitter) * slotW;
        float maxAmp = Mathf.Max(0f, slotW * 0.5f - enemyHalfWidth - Mathf.Abs(offset));

        GameObject go = Instantiate(enemyPrefab,
            new Vector3(center + offset, transform.position.y, 0f), Quaternion.identity);

        go.GetComponent<EnemyMover>().Init(rowY[row], maxAmp);
        go.GetComponent<EnemyShooter>().Init(bulletPool);

        EnemyLife enemy = go.GetComponent<EnemyLife>();
        enemy.Died += _ => occupants[slot] = null;

        occupants[slot] = enemy;
        slotRow[slot] = row;
        lastSlot = slot;
        return true;
    }

    // Adjacent columns never share a stop row
    private int PickRow(int slot)
    {
        rowCandidates.Clear();
        for (int r = 0; r < rowY.Length; r++)
        {
            bool left = slot > 0 && occupants[slot - 1] != null && slotRow[slot - 1] == r;
            bool right = slot < slotCount - 1 && occupants[slot + 1] != null && slotRow[slot + 1] == r;
            if (!left && !right) rowCandidates.Add(r);
        }
        return rowCandidates.Count > 0 ? rowCandidates[Random.Range(0, rowCandidates.Count)] : -1;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 c = transform.position;
        float w = 2f * railHalfWidth / Mathf.Max(1, slotCount);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(c + Vector3.left * railHalfWidth, c + Vector3.right * railHalfWidth);
        for (int i = 0; i <= slotCount; i++)
        {
            Vector3 p = c + Vector3.left * railHalfWidth + Vector3.right * (i * w);
            Gizmos.DrawLine(p + Vector3.up * 0.3f, p + Vector3.down * 0.3f);
        }

        Gizmos.color = Color.cyan;
        foreach (float y in rowY)
            Gizmos.DrawLine(new Vector3(c.x - railHalfWidth, y, 0f), new Vector3(c.x + railHalfWidth, y, 0f));
    }
}