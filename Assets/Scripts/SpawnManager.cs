using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject powerupPrefab;

    private int currentLevel = 1;
    private int enemiesPerLevel = 3;
    private readonly float spawnRange = 9;
    private readonly float startDelay = 1f;
    private static readonly WaitForSeconds _waitForSeconds5 = new WaitForSeconds(5f);

    public int enemyCount;
    public bool isGameOver;

    void Start()
    {
        StartCoroutine(LevelLoop());
    }

    IEnumerator LevelLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (!isGameOver)
        {
            yield return StartCoroutine(SpawnLevel());

            // Wait until all enemies are gone
            yield return new WaitUntil(() => enemyCount == 0 || isGameOver);

            if (isGameOver)
                break;

            currentLevel++;
            // Increase enemies for next level
            enemiesPerLevel += 2;

            // Delay before next level starts
            yield return _waitForSeconds5;
        }

        Debug.Log("Game Over");
    }

    IEnumerator SpawnLevel()
    {
        Debug.Log("Starting Level " + currentLevel);

        SpawnPowerup();

        for (int i = 0; i < enemiesPerLevel; i++)
        {
            if (isGameOver)
                yield break;

            SpawnEnemy();

            // Delay between enemy spawns
            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    void SpawnEnemy()
    {
        Vector3 randomPos = GenerateSpawnPosition();

        Instantiate(enemyPrefab, randomPos, enemyPrefab.transform.rotation);

        enemyCount++;
    }

    void SpawnPowerup()
    {
        Vector3 randomPos = GenerateSpawnPosition();

        Instantiate(powerupPrefab, randomPos, powerupPrefab.transform.rotation);
    }

    Vector3 GenerateSpawnPosition() {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }
}
