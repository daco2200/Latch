using UnityEngine;
using System.Collections.Generic;

public class EnemyBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int size = 24;

    [Header("Tuning (read-only while playing)")]
    [SerializeField] private int peakInUse;
    [SerializeField] private int droppedShots;

    private readonly List<GameObject> bullets = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < size; i++)
        {
            GameObject b = Instantiate(bulletPrefab, transform);
            b.SetActive(false);
            bullets.Add(b);
        }
    }

    public bool Fire(Vector2 position, Vector2 direction)
    {
        int inUse = 0;
        GameObject free = null;
        foreach (GameObject b in bullets)
        {
            if (b.activeSelf) inUse++;
            else if (free == null) free = b;
        }

        if (free == null) { droppedShots++; return false; }

        free.transform.position = position;
        free.transform.up = direction;
        free.SetActive(true);
        peakInUse = Mathf.Max(peakInUse, inUse + 1);
        return true;
    }
}