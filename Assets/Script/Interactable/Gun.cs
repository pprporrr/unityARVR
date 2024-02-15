using UnityEngine;
using System.Collections;

public class Gun : Interactable
{
    [SerializeField] private GameObject gun;
    private bool collected;
    private GameObject player;
    public PlayerHealth playerHealth;
    public PlayerUI playerUI;
    public int damage = 10;
    private Coroutine destroyCoroutine;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerUI = player.GetComponent<PlayerUI>();
        }
        collected = gun.transform.parent == player.transform;
    }

    protected override void Interact()
    {
        if (collected)
        {
            DropGun();
        }
        else
        {
            PickUpGun();
        }
    }

    private void PickUpGun()
    {
        collected = true;

        if (player != null)
        {
            gun.transform.parent = player.transform;
            playerUI.UpdateText("Pick up a " + gun.name + ".");
            switch (gun.name)
            {
                case "p9(Clone)":
                    gun.transform.localPosition = new Vector3(0.25f, 0.35f, 0f);
                    break;
                case "ar15(Clone)":
                    gun.transform.localPosition = new Vector3(0.2f, 0.65f, 0.125f);
                    break;
                case "xma10(Clone)":
                    gun.transform.localPosition = new Vector3(0.25f, 0.65f, 0.15f);
                    break;
                default:
                    gun.transform.localPosition = Vector3.zero;
                    break;
            }
            foreach (Transform child in gun.transform)
            {
                if (child.CompareTag("Particle"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void DropGun()
    {
        collected = false;

        if (player != null)
        {
            gun.transform.parent = null;
            playerUI.UpdateText("Drop a " + gun.name + ".");
            destroyCoroutine = StartCoroutine(DestroyGunAfterDelay(10f));
        }
    }

    IEnumerator DestroyGunAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!collected)
        {
            Destroy(gun);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CrystalHeart")
        {
            if (collected)
            {
                playerHealth.HandleCrystalHeartCollision(collision.gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collected)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}