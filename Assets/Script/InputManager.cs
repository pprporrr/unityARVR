using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Public variables
    public PlayerInput.OnFootActions onFoot; // Action map for on-foot actions

    // Private variables
    private PlayerInput playerInput;
    private PlayerMotor motor; // Player motor component
    private PlayerLook look; // Player look component

    void Awake()
    {
        // Initialize player input and action maps
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        // Get player motor and look components
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        // Subscribe to jump and sprint actions
        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Sprint.performed += ctx => motor.Sprint();
    }

    void FixedUpdate()
    {
        // Process player movement and look if the game state is in gameplay and right mouse button is not pressed
        if (GameManager.instance.gameState == GameState.Gameplay)
        {
            motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
            look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
        }
    }

    void OnEnable()
    {
        // Enable input actions
        onFoot.Enable();
    }

    void OnDisable()
    {
        // Disable input actions
        onFoot.Disable();
    }

    // Method to enable player movement
    public void EnablePlayerMovement()
    {
        // Enable player movement actions
        onFoot.Enable();
    }

    // Method to disable player movement
    public void DisablePlayerMovement()
    {
        // Disable player movement actions
        onFoot.Disable();
    }
}