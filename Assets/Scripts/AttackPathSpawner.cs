using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPathSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject attackPrefab;
    public float spawnInterval = 30f;
    public Vector2 spawnPosition = new Vector2(-10, 0);
    [Header("Randomization")]
    public float yOffsetRange = 3f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnAttack();
        }
    }

    private void SpawnAttack()
    {
        if (attackPrefab == null) return;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        float randomY = Random.Range(-yOffsetRange, yOffsetRange);
        Vector3 spawnPos = new Vector3(leftEdge.x + 10f, randomY, 0);

        Instantiate(attackPrefab, spawnPos, Quaternion.identity);
    }
}
