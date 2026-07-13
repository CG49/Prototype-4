using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 30f;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private PowerUpType currentPowerUp = PowerUpType.None;
    [SerializeField] private float hangTime;
    [SerializeField] private float smashSpeed;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadius;

    private float floorY;
    private bool hasPowerup;
    private bool isSmashing;
    private readonly float outOfBounds = -5f;
    private readonly float powerUpStrength = 18f;

    private InputSystem_Actions controls;
    private InputSystem_Actions.PlayerActions playerActions;

    private Rigidbody playerRb;
    private GameObject focalPoint;
    private SpawnManager spawnManager;

    private Coroutine powerupCountdown;

    void Awake()
    {
        controls = new InputSystem_Actions();

        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.FindGameObjectWithTag("FocalPoint");
        spawnManager = GameObject.FindWithTag("Respawn").GetComponent<SpawnManager>();

        playerActions = controls.Player;
    }

    void OnEnable()
    {
        playerActions.Enable();
        playerActions.Fire.performed += OnFire;
        playerActions.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        playerActions.Fire.performed -= OnFire;
        playerActions.Jump.performed -= OnJump;
        playerActions.Disable();
    }

    void OnFire(InputAction.CallbackContext context)
    {
        if (currentPowerUp == PowerUpType.Rockets)
        {
            LaunchRockets();
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (!isSmashing && currentPowerUp == PowerUpType.Smash)
        {
            isSmashing = true;
            StartCoroutine(Smash());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(spawnManager.powerUpTimeLimit);

        hasPowerup = false;
        currentPowerUp = PowerUpType.None;

        powerupIndicator.SetActive(false);
    }

    IEnumerator Smash()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>();

        // Store the y position before taking off
        floorY = transform.position.y;

        // Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            // move the player up while still keeping their x velocity.
            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, smashSpeed, playerRb.linearVelocity.z);
            yield return null;
        }

        // Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, -smashSpeed * 2, playerRb.linearVelocity.z);
            yield return null;
        }

        // Cycle through all enemies.
        for (int i = 0; i < enemies.Length; i++)
        {
            // Apply an explosion force that originates from our position.
            if (enemies[i] != null)
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }

        // We are no longer smashing, so set the boolean to false
        isSmashing = false;
    }

    void Update()
    {
        if (transform.position.y < outOfBounds)
        {
            spawnManager.isGameOver = true;
        }
    }

    void FixedUpdate()
    {
        if (isSmashing)
            return;

        Vector2 moveInput = playerActions.Move.ReadValue<Vector2>();

        float forwardInput = moveInput.y;

        playerRb.AddForce(forwardInput * playerSpeed * focalPoint.transform.forward);
    }

    void LateUpdate()
    {
        powerupIndicator.transform.position = transform.position + Vector3.down * 0.5f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Powerup"))
            return;

        hasPowerup = true;
        currentPowerUp = other.GetComponent<PowerUp>().powerUpType;

        Destroy(other.gameObject);

        powerupIndicator.SetActive(true);

        if (powerupCountdown != null)
        {
            StopCoroutine(powerupCountdown);
        }

        powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isSmashing)
            return;

        if (!hasPowerup)
            return;

        if (!collision.gameObject.CompareTag("Enemy"))
            return;

        if (currentPowerUp != PowerUpType.Pushback)
            return;

        Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

        Vector3 awayFromPlayer = collision.transform.position - transform.position;

        enemyRb.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
    }

    void LaunchRockets()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            GameObject rocket = Instantiate(
                rocketPrefab,
                transform.position + Vector3.up,
                Quaternion.identity
            );

            rocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }
}
