using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private bool hasPowerup;
    [SerializeField] private float playerSpeed = 25f;

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

    void FixedUpdate()
    {
        Vector2 moveInput = playerActions.Move.ReadValue<Vector2>();

        float forwardInput = moveInput.y;

        playerRb.AddForce(forwardInput * playerSpeed * focalPoint.transform.forward);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);
        }
    }
}
