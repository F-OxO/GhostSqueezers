using UnityEngine;
using System.Collections;

public class GhostSpawner : MonoBehaviour
{
    public GameObject minighost1Prefab;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;

    public float spawnRadius = 3f;
    public float minHeight = 0f;
    public float maxHeight = 2f;

    public int ghostsPerWave = 10;
    public float delayBetweenGhosts = 0.1f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait between waves
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Spawn a full wave
            yield return StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < ghostsPerWave; i++)
        {
            SpawnGhost();
            yield return new WaitForSeconds(delayBetweenGhosts);
        }
    }

    void SpawnGhost()
    {
        if (minighost1Prefab == null)
        {
            Debug.LogWarning("Missing prefab!");
            return;
        }

        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 offset = new Vector3(circle.x, Random.Range(minHeight, maxHeight), circle.y);
        Vector3 spawnPosition = transform.position + offset;

        GameObject ghost = Instantiate(minighost1Prefab, spawnPosition, Quaternion.identity);
        ghost.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
}
