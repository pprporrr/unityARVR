using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Public variables
    public float movementSpeed = 0.25f;
    public int damage = 10;
    public float checkDistance = 0.1f;
    public LayerMask sceneLayer;

    // Private variables
    private Transform player;
    private int currentHealth;
    private string prefabName;
    private Rigidbody rb;
    private GameManager gameManager;
    public int maxHealth;
    private bool isFalling = false;

    void Start()
    {
        // Initialization
        prefabName = gameObject.name;
        gameManager = GameManager.instance;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Check if player is found
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }

        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Check if enemy is falling
        if (!isFalling && transform.position.y < -10f)
        {
            DieByFalling();
        }

        // Move enemy towards player if not falling
        if (player != null && !isFalling)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            RaycastHit hit;

            // Raycast to detect obstacles
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * checkDistance, Color.red, 0.1f);

            if (Physics.Raycast(ray, out hit, checkDistance, sceneLayer))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    direction = FindNewDirection();
                }
            }
            transform.Translate(direction * movementSpeed * Time.deltaTime);
        }
    }

    // Method to find a new direction when obstructed by a wall
    Vector3 FindNewDirection()
    {
        Vector3 randomDirection = new Vector3(0f, Random.Range(-1f, 1f), 0f).normalized;
        return randomDirection;
    }

    // Method to handle taking damage
    void TakeDamage()
    {
        currentHealth -= 10;
        if (currentHealth <= 0)
        {
            DieByBullet();
        }
    }

    // Method to handle enemy death by bullet
    void DieByBullet()
    {
        gameObject.SetActive(false);
        movementSpeed = 0.25f;
        GameManager.instance.EnemyDiedByBullet(gameObject);
    }

    // Method to handle enemy death by falling
    void DieByFalling()
    {
        gameObject.SetActive(false);
        movementSpeed = 0.25f;
        GameManager.instance.EnemyDiedByFalling(gameObject);
    }

    // Method to handle collisions with other objects
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            movementSpeed = 0f;
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage();
            collision.gameObject.SetActive(false);
        }
    }

    // Method to handle exit of collision with player
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            movementSpeed = 0.25f;
        }
    }
}