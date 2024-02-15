using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Function called when a collision is detected
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the wall is tagged as "Bullet"
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Disable the wall object
            collision.gameObject.SetActive(false);
        }
    }
}
