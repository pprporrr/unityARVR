using UnityEngine;
using System.Collections.Generic;

public class GunShooting : MonoBehaviour
{
    public ObjectPoolManager objectPoolManager;
    public PlayerUI playerUI;
    public InputManager inputManager;
    public float bulletForce = 7f;
    public float reloadTime = 2f;
    private bool isReloading = false;
    public AudioSource audioSourceP9;
    public AudioClip shootSoundP9;
    public AudioSource audioSourceAr15;
    public AudioClip shootSoundAr15;
    public AudioSource audioSourceXma10;
    public AudioClip shootSoundXma10;
    public AudioSource audioSourceReload;
    public AudioClip reloadSound;
    private Dictionary<string, int> gunBulletsAvailable = new Dictionary<string, int>();
    private GameManager gameManager;
    public GameObject particlePrefab;

    void Start()
    {
        gameManager = GameManager.instance;
        gunBulletsAvailable["p9(Clone)"] = 5;
        gunBulletsAvailable["ar15(Clone)"] = 7;
        gunBulletsAvailable["xma10(Clone)"] = 3;
        string gunName;
        if (IsGunAttachedToPlayer(out gunName))
        {
            playerUI.UpdateText(gunName + " has " + gunBulletsAvailable[gunName] + " bullets.");
        }
        // Check if the GameManager instance is not null
        if (gameManager != null)
        {
            // Access the gameState variable
            GameState gameState = gameManager.gameState;
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    void Update()
    {
        GameState gameState = gameManager.gameState;
        string gunName;
        if (gameState.ToString() == "Gameplay")
        {
            if (IsGunAttachedToPlayer(out gunName) && inputManager.onFoot.Shoot.triggered && !isReloading && gunBulletsAvailable[gunName] > 0)
            {
                if (objectPoolManager == null)
                {
                    Debug.LogError("ObjectPoolManager is not assigned!");
                    return;
                }

                Shoot(gunName);
                if (gunName == "p9(Clone)")
                {
                    audioSourceP9.PlayOneShot(shootSoundP9);
                } else if (gunName == "ar15(Clone)")
                {
                    audioSourceAr15.PlayOneShot(shootSoundAr15);
                } else if (gunName == "xma10(Clone)")
                {
                    audioSourceXma10.PlayOneShot(shootSoundXma10);
                }
                gunBulletsAvailable[gunName]--;
                playerUI.UpdateText(gunName + " has " + gunBulletsAvailable[gunName] + " bullets.");
            }

            if (IsGunAttachedToPlayer(out gunName) && inputManager.onFoot.Reload.triggered && !isReloading)
            {
                Reload(gunName);
            }
        }
    }

    void Shoot(string gunName)
    {
        GameObject bullet = objectPoolManager.GetBullet(gunName);
        if (bullet != null)
        {
            // Set the position and rotation of the bullet to match the gun's position and rotation
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            // Activate the bullet
            bullet.SetActive(true);

            // Get the rigidbody component of the bullet
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            // Apply force to shoot the bullet forward
            if (bulletRigidbody != null)
            {
                GameObject particleObject = Instantiate(particlePrefab, bullet.transform.position, Quaternion.identity);
                particleObject.transform.parent = bullet.transform;
                bulletRigidbody.velocity = transform.forward * bulletForce;
            }
            else
            {
                Debug.LogError("Bullet prefab does not have a Rigidbody component attached!");
            }
        }
    }

    void Reload(string gunName)
    {
        isReloading = true;

        // Determine reload time based on the gun's name
        float gunReloadTime = GetReloadTime(gunName);
        
        Invoke("FinishReload", gunReloadTime);
        playerUI.UpdateText("Reloading...");
    }

    float GetReloadTime(string gunName)
    {
        switch (gunName)
        {
            case "p9(Clone)":
                return 2.0f; // Reload time for p9
            case "ar15(Clone)":
                return 2.5f; // Reload time for ar15
            case "xma10(Clone)":
                return 1.5f; // Reload time for xma10
            default:
                Debug.LogWarning("Unknown gun type: " + gunName);
                return reloadTime; // Return default reload time
        }
    }

    void FinishReload()
    {
        isReloading = false;
        string gunName;
        audioSourceReload.PlayOneShot(reloadSound);
        if (IsGunAttachedToPlayer(out gunName))
        {
            // Set the bullets available to the initial count for the specific gun
            switch (gunName)
            {
                case "p9(Clone)":
                    gunBulletsAvailable[gunName] = 5;
                    break;
                case "ar15(Clone)":
                    gunBulletsAvailable[gunName] = 7;
                    break;
                case "xma10(Clone)":
                    gunBulletsAvailable[gunName] = 3;
                    break;
                default:
                    Debug.LogWarning("Unknown gun type: " + gunName);
                    break;
            }
            //Debug.Log("Reload finished. " + gunName + " has " + gunBulletsAvailable[gunName] + " bullets.");
            playerUI.UpdateText("Reload finished!");
        }
    }

    bool IsGunAttachedToPlayer(out string gunName)
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Gun"))
            {
                gunName = child.name;
                return true;
            }
        }

        gunName = null;
        return false;
    }

}