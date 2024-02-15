using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public Image sprintingButton;
    private bool isGrounded;
    private bool sprinting;
    private float speed = 1.25f;
    private float gravity = -9.8f;
    private float jumpHeight = 0.2f;
    private float maxStamina;
    private float currentStamina;
    public float chipSpeed = 0.75f;
    public Image frontStaminaBar;
    public Image backStaminaBar;
    private float lerpTimer;
    private bool adjustedMaxStamina = false;
    public float staminaDepletionRate = 0.25f;
    public float staminaRegenerationRate;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (!adjustedMaxStamina)
        {
            foreach (Transform child in transform)
            {
                string childName = child.gameObject.name;
                if (childName.Contains("Pawn(Clone)"))
                {
                    maxStamina = 3.5f;
                    staminaRegenerationRate = 0.5f;
                    adjustedMaxStamina = true;
                }
                if (childName.Contains("Knight(Clone)"))
                {
                    maxStamina = 3f;
                    staminaRegenerationRate = 0.4f;
                    adjustedMaxStamina = true;
                }
                if (childName.Contains("Bishop(Clone)"))
                {
                    maxStamina = 2f;
                    staminaRegenerationRate = 0.35f;
                    adjustedMaxStamina = true;
                }
                else if (childName.Contains("Rook(Clone)"))
                {
                    maxStamina = 2f;
                    staminaRegenerationRate = 0.35f;
                    adjustedMaxStamina = true;
                }
                else if (childName.Contains("Queen(Clone)"))
                {
                    maxStamina = 1f;
                    staminaRegenerationRate = 0.2f;
                    adjustedMaxStamina = true;
                }
                else if (childName.Contains("King(Clone)"))
                {
                    maxStamina = 1f;
                    staminaRegenerationRate = 0.2f;
                    adjustedMaxStamina = true;
                }
            }
            currentStamina = maxStamina;
        }
        UpdateStamina();
    }

    private void UpdateStamina()
    {
        if (sprinting)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            UpdateStaminaUI();
            if (currentStamina <= 0)
            {
                sprintingButton.color = Color.white;
                sprinting = false;
                speed = 1f;
            }
        }
        else
        {
            currentStamina += staminaRegenerationRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateStaminaUI();
        }
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        if (sprinting && currentStamina > 0)
        {
            controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(transform.TransformDirection(moveDirection) * speed * 0.5f * Time.deltaTime);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Sprint()
    {
        if (!sprinting && currentStamina > 0)
        {
            sprintingButton.color = new Color(80f/255f, 80f/255f, 80f/255f, 0.8f);
            sprinting = true;
            speed = 1.25f;
        }
        else
        {
            sprintingButton.color = Color.white;
            sprinting = false;
            speed = 1f;
        }
    }

    void UpdateStaminaUI()
    {
        float fillF = frontStaminaBar.fillAmount;
        float fillB = backStaminaBar.fillAmount;
        float hFraction = (float)currentStamina / maxStamina;
        if (fillB > hFraction)
        {
            frontStaminaBar.fillAmount = hFraction;
            backStaminaBar.color = Color.red;
            lerpTimer = 2f; // Reset the timer when Stamina decreases
            float percentComplete = Mathf.Clamp01(lerpTimer / chipSpeed); // Ensure it doesn't go over 1
            backStaminaBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        } 
        if (fillF < hFraction) 
        {
            backStaminaBar.color = Color.green;
            backStaminaBar.fillAmount = hFraction;
            lerpTimer = 2f; // Reset the timer when Stamina increases
            float percentComplete = Mathf.Clamp01(lerpTimer / chipSpeed); // Ensure it doesn't go over 1
            frontStaminaBar.fillAmount = Mathf.Lerp(fillF, backStaminaBar.fillAmount, percentComplete);
        }
    }

    // Method to remove the "gun" tag from children, move parent to (0, 0, 0), and disable the parent
    public void ResetPlayer()
    {
        // Remove the "gun" tag from all children objects
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("Gun"))
            {
                Destroy(child.gameObject);
            }
        }
        currentStamina = maxStamina;
        UpdateStaminaUI();

        // Move the parent object to position (0, 0, 0)
        transform.parent.position = Vector3.zero;
        transform.position = Vector3.zero;

        // Disable the parent object
        transform.parent.gameObject.SetActive(false);
    }
}