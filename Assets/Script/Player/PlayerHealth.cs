using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public AudioSource audioSourceHeal;
    public AudioClip healSound;
    public AudioSource audioSourceHurt;
    public AudioClip hurtSound;
    public float chipSpeed = 0.75f;
    public Image frontHealthBar;
    public Image backHealthBar;
    private float lerpTimer;
    private bool adjustedMaxHealth = false;
    void Start()
    {
        audioSourceHeal = GameObject.Find("AudioHeal").GetComponent<AudioSource>();
        audioSourceHurt = GameObject.Find("AudioHurt").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!adjustedMaxHealth)
        {
            foreach (Transform child in transform)
            {
                string childName = child.gameObject.name;
                if (childName.Contains("Pawn(Clone)"))
                {
                    maxHealth = 100;
                    currentHealth = 100;
                    adjustedMaxHealth = true;
                }
                else if (childName.Contains("Knight(Clone)"))
                {
                    maxHealth = 125;
                    currentHealth = 125;
                    adjustedMaxHealth = true;
                }
                else if (childName.Contains("Bishop(Clone)"))
                {
                    maxHealth = 150;
                    currentHealth = 150;
                    adjustedMaxHealth = true;
                }
                else if (childName.Contains("Rook(Clone)"))
                {
                    maxHealth = 150;
                    currentHealth = 150;
                    adjustedMaxHealth = true;
                }
                else if (childName.Contains("Queen(Clone)"))
                {
                    maxHealth = 200;
                    currentHealth = 200;
                    adjustedMaxHealth = true;
                }
                else if (childName.Contains("King(Clone)"))
                {
                    maxHealth = 200;
                    currentHealth = 200;
                    adjustedMaxHealth = true;
                }
            }
            Debug.Log("Maxhealth: " + maxHealth);
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
        audioSourceHurt.PlayOneShot(hurtSound);
        currentHealth -= damageAmount;
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die();
        }
        
    }

    void Die()
    {
        Debug.Log("Player has died!");
        GameManager.instance.EndGame();
    }

    void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = (float)currentHealth / maxHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer = 2f; // Reset the timer when health decreases
            float percentComplete = Mathf.Clamp01(lerpTimer / chipSpeed); // Ensure it doesn't go over 1
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        } 
        if (fillF < hFraction) 
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer = 2f; // Reset the timer when health increases
            float percentComplete = Mathf.Clamp01(lerpTimer / chipSpeed); // Ensure it doesn't go over 1
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CrystalHeart")
        {
            HandleCrystalHeartCollision(collision.gameObject);
        }
    }

    public void HandleCrystalHeartCollision(GameObject crystalHeart)
    {
        audioSourceHeal.PlayOneShot(healSound);
        if (currentHealth < maxHealth)
        {
            currentHealth += 20;
        }
        Destroy(crystalHeart);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
}