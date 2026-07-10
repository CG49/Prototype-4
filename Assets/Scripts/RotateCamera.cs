using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 80f;

    private InputSystem_Actions controls;
    private InputSystem_Actions.PlayerActions playerActions;

    void Awake()
    {
        controls = new InputSystem_Actions();
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

    void Update()
    {
        Vector2 moveInput = playerActions.Move.ReadValue<Vector2>();

        float horizontalInput = moveInput.x;

        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
