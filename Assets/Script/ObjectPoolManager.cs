using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // Singleton instance
    public static ObjectPoolManager instance;

    // Prefabs and pool sizes
    public GameObject p9BulletPrefab;
    public GameObject ar15BulletPrefab;
    public GameObject xma10BulletPrefab;
    public GameObject kingEnemyPrefab;
    public GameObject queenEnemyPrefab;
    public GameObject rookEnemyPrefab;
    public GameObject bishopEnemyPrefab;
    public GameObject knightEnemyPrefab;
    public GameObject pawnEnemyPrefab;
    public int p9BulletPoolSize = 5;
    public int ar15BulletPoolSize = 7;
    public int xma10BulletPoolSize = 3;
    public int enemyPoolSize = 13;

    // Object pools
    private List<GameObject> p9BulletPool;
    private List<GameObject> ar15BulletPool;
    private List<GameObject> xma10BulletPool;
    private List<GameObject> enemyPool;

    // Materials for objects
    public Material[] materials;

    // Threshold for disabling bullets
    public float bulletDistanceThreshold = 10f;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        // Initialize object pools
        p9BulletPool = new List<GameObject>();
        ar15BulletPool = new List<GameObject>();
        xma10BulletPool = new List<GameObject>();
        enemyPool = new List<GameObject>();

        InitializePool(p9BulletPrefab, p9BulletPoolSize, p9BulletPool);
        InitializePool(ar15BulletPrefab, ar15BulletPoolSize, ar15BulletPool);
        InitializePool(xma10BulletPrefab, xma10BulletPoolSize, xma10BulletPool);

        // Add enemies to the enemy pool
        InitializeEnemyPool();
        
        // Get selected material
        int selectedMaterialIndex = PlayerPrefs.GetInt("selectedMaterial");
        Material selectedMat;

        // Adjust the index to access the correct material in the array
        if (selectedMaterialIndex == 0)
        {
            selectedMat = materials[1];
        }
        else if (selectedMaterialIndex == 1)
        {
            selectedMat = materials[0];
        }
        else
        {
            selectedMat = materials[0];
        }

        // Apply selected material to enemies
        foreach (GameObject enemy in enemyPool)
        {
            Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = selectedMat;
            }
        }
    }

    // Method to initialize enemy pool with specified enemies
    private void InitializeEnemyPool()
    {
        AddEnemiesToPool("KingEnemy", 1);
        AddEnemiesToPool("QueenEnemy", 1);
        AddEnemiesToPool("RookEnemy", 2);
        AddEnemiesToPool("BishopEnemy", 2);
        AddEnemiesToPool("KnightEnemy", 2);
        AddEnemiesToPool("PawnEnemy", 5);
    }

    // Method to add enemies to the pool
    private void AddEnemiesToPool(string enemyType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemyPrefab = GetEnemyPrefab(enemyType);
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    // Method to get the appropriate enemy prefab based on the type
    private GameObject GetEnemyPrefab(string enemyType)
    {
        switch (enemyType)
        {
            case "KingEnemy":
                return kingEnemyPrefab;
            case "QueenEnemy":
                return queenEnemyPrefab;
            case "RookEnemy":
                return rookEnemyPrefab;
            case "BishopEnemy":
                return bishopEnemyPrefab;
            case "KnightEnemy":
                return knightEnemyPrefab;
            case "PawnEnemy":
                return pawnEnemyPrefab;
            default:
                Debug.LogError("Unknown enemy type: " + enemyType);
                return null;
        }
    }

    // Method to initialize object pool
    private void InitializePool(GameObject prefab, int size, List<GameObject> pool)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    // Method to get bullet from the pool based on weapon type
    public GameObject GetBullet(string weaponName)
    {
        switch (weaponName)
        {
            case "p9(Clone)":
                return GetObjectFromPool(p9BulletPool);
            case "ar15(Clone)":
                return GetObjectFromPool(ar15BulletPool);
            case "xma10(Clone)":
                return GetObjectFromPool(xma10BulletPool);
            default:
                return null;
        }
    }

    // Method to get enemy from the pool by prefab name
    public GameObject GetEnemy(string enemyPrefabName)
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.name.Equals(enemyPrefabName))
            {
                if (!enemy.activeInHierarchy)
                {
                    enemy.SetActive(true);
                    return enemy;
                }
            }
        }
        return null; // If no inactive objects found or the provided name is invalid
    }

    // Method to retrieve object from the pool
    private GameObject GetObjectFromPool(List<GameObject> pool)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null; // If no inactive objects found
    }

    // Method to reset the object pool manager state
    public void ResetState()
    {
        DisableAllObjectsInPool(); // Disable all objects in the pool
    }

    // Method to disable all objects in the pool
    public void DisableAllObjectsInPool()
    {
        foreach (GameObject obj in p9BulletPool)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in ar15BulletPool)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in xma10BulletPool)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in enemyPool)
        {
            obj.SetActive(false);
        }
    }

    // Method to get the number of enemies to spawn
    public int GetNumberOfEnemiesToSpawn()
    {
        return enemyPoolSize;
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndDisableBullets();
    }

    // Check if bullets are far away and disable them
    void CheckAndDisableBullets()
    {
        foreach (GameObject bullet in p9BulletPool)
        {
            if (bullet.activeInHierarchy && Vector3.Distance(bullet.transform.position, Vector3.zero) > bulletDistanceThreshold)
            {
                bullet.SetActive(false);
            }
        }
        foreach (GameObject bullet in ar15BulletPool)
        {
            if (bullet.activeInHierarchy && Vector3.Distance(bullet.transform.position, Vector3.zero) > bulletDistanceThreshold)
            {
                bullet.SetActive(false);
            }
        }
        foreach (GameObject bullet in xma10BulletPool)
        {
            if (bullet.activeInHierarchy && Vector3.Distance(bullet.transform.position, Vector3.zero) > bulletDistanceThreshold)
            {
                bullet.SetActive(false);
            }
        }
    }
}