using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    private readonly float speed = 2f;
    private readonly float outOfBounds = -10f;

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
    }

    void OnDestroy()
    {
        if (spawnManager != null)
        {
            spawnManager.enemyCount--;
        }
    }
}

