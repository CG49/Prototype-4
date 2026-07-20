using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private bool isBoss;
    [SerializeField] private float speed = 2f;

    private readonly float outOfBounds = -10f;
    public float spawnInterval;
    private float nextSpawn;

    private GameObject player;
    private Rigidbody enemyRb;
    private SpawnManager spawnManager;

    void Awake()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");

        spawnManager = GameObject.FindWithTag("Respawn").GetComponent<SpawnManager>();
    }

    void Update()
    {
        if (transform.position.y < outOfBounds)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;

        enemyRb.AddForce(lookDirection * speed);

        if (isBoss)
        {
            if (Time.time > nextSpawn)
            {
                nextSpawn = Time.time + spawnInterval;
                spawnManager.SpawnMiniEnemy(spawnManager.GetMiniEnemyAmount());
            }
        }
    }

    void OnDestroy()
    {
        if (spawnManager != null)
        {
            spawnManager.enemyCount--;
        }
    }
}
