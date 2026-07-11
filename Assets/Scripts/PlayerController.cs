using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 30f;
    [SerializeField] private GameObject powerupIndicator;

    private bool hasPowerup;
    private readonly float powerUpStrength = 18f;
    private readonly float powerUpTimeLimit = 10f;

    private InputSystem_Actions controls;
    private InputSystem_Actions.PlayerActions playerActions;

    private Rigidbody playerRb;
    private GameObject focalPoint;

    void Awake()
    {
        controls = new InputSystem_Actions();

        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.FindGameObjectWithTag("FocalPoint");

        playerActions = controls.Player;
    }

    void OnEnable()
    {
        playerActions.Enable();
    }

    void OnDisable()
    {
        playerActions.Disable();
    }

    IEnumerator PowerupCountdownRoutine() {
        yield return new WaitForSeconds(powerUpTimeLimit);

        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    void FixedUpdate()
    {
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
        if(other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);

            powerupIndicator.SetActive(true);

            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRB = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            enemyRB.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
        }
    }
}
