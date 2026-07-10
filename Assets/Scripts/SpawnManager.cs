using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private float spawnRange = 9;

    private readonly float startDelay = 1f;
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }
    }

    void SpawnEnemy()
    {
        Vector3 randomPos = GenerateSpawnPosition();

        Instantiate(enemyPrefab, randomPos, enemyPrefab.transform.rotation);
    }

    Vector3 GenerateSpawnPosition() {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }
}
