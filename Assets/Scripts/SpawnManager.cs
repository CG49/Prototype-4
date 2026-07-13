using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int bossRound;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] powerupPrefabs;
    [SerializeField] private GameObject[] miniEnemyPrefabs;

    private int currentLevel = 1;
    private int enemiesPerLevel = 2;
    private readonly float enemySpawnRange = 9;
    private readonly float powerUpSpawnRange = 4;
    private readonly float startDelay = 1f;
    private static readonly WaitForSeconds _waitForSeconds5 = new WaitForSeconds(5f);

    public int enemyCount;
    public bool isGameOver;
    public float powerUpTimeLimit = 10f;

    private Rigidbody playerRb;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
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

            // Reset player position
            player.position = Vector3.zero;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;

            currentLevel++;
            enemiesPerLevel += 2;
            powerUpTimeLimit += Random.Range(8, 10);

            // Delay before next level starts
            yield return _waitForSeconds5;
        }

        Debug.Log("Game Over");
    }

    IEnumerator SpawnLevel()
    {
        Debug.Log("Starting Level " + currentLevel);

        SpawnPowerup();

        if (bossRound > 0 && currentLevel % bossRound == 0)
        {
            SpawnBossWave(currentLevel);
            yield break;
        }

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
        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        Vector3 randomPos = GenerateSpawnPosition(enemySpawnRange);

        Instantiate(enemyPrefabs[randomEnemy], randomPos, enemyPrefabs[randomEnemy].transform.rotation);

        enemyCount++;
    }

    void SpawnPowerup()
    {
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);
        Vector3 randomPos = GenerateSpawnPosition(powerUpSpawnRange);

        Instantiate(powerupPrefabs[randomPowerup], randomPos, powerupPrefabs[randomPowerup].transform.rotation);
    }

    void SpawnBossWave(int currentRound)
    {
        Instantiate(bossPrefab, GenerateSpawnPosition(enemySpawnRange), bossPrefab.transform.rotation);

        enemyCount++;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);

            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(enemySpawnRange), miniEnemyPrefabs[randomMini].transform.rotation);

            enemyCount++;
        }
    }

    public int GetMiniEnemyAmount()
    {
        return bossRound > 0 ? currentLevel / bossRound : 1;
    }

    Vector3 GenerateSpawnPosition(float spawnRange) {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }
}
