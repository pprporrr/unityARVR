using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float distance = 1f;
    [SerializeField] private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the forward direction of the player
        Vector3 playerForward = transform.forward;
        Vector3 playerRight = transform.right;

        Ray frontRay = new Ray(transform.position, playerForward);
        
        // Shift the right ray by 0.5 units upwards in the y-axis
        Vector3 rightRayOrigin = transform.position + new Vector3(0f, 0.15f, 0f);
        Ray rightRay = new Ray(rightRayOrigin, playerRight);

        Debug.DrawRay(frontRay.origin, frontRay.direction * distance, Color.red, 0.1f);
        Debug.DrawRay(rightRay.origin, rightRay.direction * distance, Color.blue, 0.1f);

        RaycastHit hitInfo;

        if (Physics.Raycast(frontRay, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
        else if (Physics.Raycast(rightRay, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}