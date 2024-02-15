using UnityEngine;
using TMPro;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance;

    // Game state
    public GameState gameState;
    public GameObject heartPrefab;
    public int dropRNG = 0;

    // UI elements
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text diedText;
    public TMP_Text diedScoreText;
    public GameObject scoreTextBG;
    public GameObject timeTextBG;
    public GameObject interactTextBG;
    public GameObject LoggerContainer;
    public GameObject Bar;

    // Gameplay parameters
    public float gameplayDuration = 60f;
    private float currentTime = 0f;
    private int score = 0;
    private int rockDropCount = 0;

    // GameObject references
    public GameObject OnScreenStick;
    public GameObject Screen;
    public GameObject restartButton;
    public ObjectPoolManager objectPoolManager;
    public GameObject planeScene;
    public GameObject player;
    public GameObject gunSpawnPrefab;
    public GameObject[] characterPrefabs;
    public GameObject[] weaponPrefabs;
    public GameObject[] rockPrefabs;
    private List<GameObject> instantiatedGuns = new List<GameObject>();
    private List<GameObject> instantiatedHearts = new List<GameObject>();
    private List<GameObject> instantiatedRocks = new List<GameObject>();
    // Control variables
    private bool navMeshBuilt = false;
    private bool knightSpawned = false;
    private bool bishopSpawned = false;
    private bool rookSpawned = false;
    private bool queenSpawned = false;
    private bool kingSpawned = false;
    private InputManager inputManager;

    private int enemyPoolNumber;
    public AudioSource audioSource;
    public AudioClip heartDropSound;
    public AudioSource audioSourceBG1;
    public AudioClip soundBG1;
    public AudioSource audioSourceBG2;
    public AudioClip soundBG2;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Set initial game state
        gameState = GameState.Scanning;
        audioSource = GameObject.Find("AudioHeartDrop").GetComponent<AudioSource>();
        inputManager = FindObjectOfType<InputManager>();
    }

    void Update()
    {
        // Update gameplay only when in Gameplay state
        if (gameState == GameState.Gameplay)
        {
            // Update time
            currentTime += Time.deltaTime;
            if (currentTime >= gameplayDuration)
                EndGame(); // End game if time is up
            UpdateTimeText(); // Update time UI

            // Build NavMesh once if not already built
            if (!navMeshBuilt){
                BuildNavMesh();
            }

            // Check if player falls
            if (player.transform.position.y < -10f) // Adjust threshold as per your scene
            {
                EndGame(); // Game over if the player falls below a certain Y position
            }
            // Check if audio source needs to be switched
            if (!audioSourceBG2.isPlaying)
            {
                audioSourceBG2.Play();
            }
        }
        else if (gameState == GameState.Ended)
        {
            // Check if audio source needs to be switched
            if (!audioSourceBG1.isPlaying)
            {
                audioSourceBG1.Play();
            }
            if (audioSourceBG2.isPlaying)
            {
                audioSourceBG2.Stop();
            }
        }
    }

    // Method to build NavMesh
    private void BuildNavMesh()
    {
        if (planeScene != null)
        {
            while (rockDropCount < 4)
            {
                DropRockOnPlane();
                rockDropCount++;
            }
            // Add NavMeshSurface if not present
            NavMeshSurface navMeshSurface = planeScene.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
                navMeshSurface = planeScene.gameObject.AddComponent<NavMeshSurface>();

            // Build NavMesh only on the "Plane" layer
            int layerMask = 1 << LayerMask.NameToLayer("Plane");
            navMeshSurface.layerMask = layerMask;
            navMeshSurface.BuildNavMesh();

            // Disable other planes
            DisableOtherPlanes(planeScene);

            // Update flag
            navMeshBuilt = true;
        } else if (planeScene == null)
        {
            planeScene = FindPlane(); // Find the largest plane in the scene
            if (planeScene != null)
            {
                while (rockDropCount < 4)
                {
                    DropRockOnPlane();
                    rockDropCount++;
                }
                // Add NavMeshSurface if not present
                NavMeshSurface navMeshSurface = planeScene.GetComponent<NavMeshSurface>();
                if (navMeshSurface == null)
                    navMeshSurface = planeScene.gameObject.AddComponent<NavMeshSurface>();

                // Build NavMesh only on the "Plane" layer
                int layerMask = 1 << LayerMask.NameToLayer("Plane");
                navMeshSurface.layerMask = layerMask;
                navMeshSurface.BuildNavMesh();

                // Disable other planes
                DisableOtherPlanes(planeScene);

                // Update flag
                navMeshBuilt = true;
            }
        }
    }

    // Method to disable other planes in the scene
    private void DisableOtherPlanes(GameObject planeToKeepActive)
    {
        GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");
        foreach (GameObject plane in planes)
        {
            if (plane != planeToKeepActive)
                plane.SetActive(false);
        }
    }

    // Update time UI
    void UpdateTimeText()
    {
        float remainingTime = Mathf.Max(0, gameplayDuration - currentTime);
        timeText.text = "Time: " + Mathf.FloorToInt(remainingTime).ToString();
    }

    // Enemy killed by bullet
    public void EnemyDiedByBullet(GameObject enemy)
    {
        score += 1;
        UpdateScoreText();
        StartCoroutine(RespawnEnemyWithDelay(enemy));
    }

    // Enemy killed by falling
    public void EnemyDiedByFalling(GameObject enemy)
    {
        StartCoroutine(RespawnEnemyWithDelay(enemy));
    }

    // Update score UI
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
        // Check if the score is a multiple of 5
        if (score % 5 == 0)
        {
            DropGunOnPlane();
        }
        dropRNG = Random.Range(0, 100);
        if (dropRNG > 75)
        {
            DropHeartOnPlane();
        }
        if (score > 10 && !knightSpawned)
        {
            StartCoroutine(SpawnEnemiesWithDelay("KnightEnemy(Clone)"));
            knightSpawned = true;
        }
        if (score > 15 && !bishopSpawned)
        {
            StartCoroutine(SpawnEnemiesWithDelay("BishopEnemy(Clone)"));
            bishopSpawned = true;
        }
        if (score > 20 && !rookSpawned)
        {
            StartCoroutine(SpawnEnemiesWithDelay("RookEnemy(Clone)"));
            rookSpawned = true;
        }
        if (score > 25 && !queenSpawned)
        {
            StartCoroutine(SpawnEnemiesWithDelay("QueenEnemy(Clone)"));
            queenSpawned = true;
        }
        if (score > 30 && !kingSpawned)
        {
            StartCoroutine(SpawnEnemiesWithDelay("KingEnemy(Clone)"));
            kingSpawned = true;
        }
    }

    // Start gameplay
    public void StartGameplay()
    {
        gameState = GameState.Gameplay;
        OnScreenStick.SetActive(true);
        scoreTextBG.SetActive(true);
        timeTextBG.SetActive(true);
        interactTextBG.SetActive(true);
        Bar.SetActive(true);
        enemyPoolNumber = objectPoolManager.GetNumberOfEnemiesToSpawn();
        SpawnWeaponForPlayer();
        StartCoroutine(SpawnEnemiesWithDelay("PawnEnemy(Clone)"));
    }

    // End game
    public void EndGame()
    {
        gameState = GameState.Ended;
        LoggerContainer.SetActive(false);
        objectPoolManager.DisableAllObjectsInPool();
        Screen.SetActive(true);
        restartButton.SetActive(true);
        scoreTextBG.SetActive(false);
        timeTextBG.SetActive(false);
        interactTextBG.SetActive(false);
        Bar.SetActive(false);
        diedText.text = "GAME OVER!";
        diedScoreText.text = "Score: " + score.ToString();
        // Stop playing audio source BG2 if it's playing
    }

    // Restart the game
    public void RestartGame()
    {
        // Stop playing audio source BG2 if it's playing
        if (audioSourceBG1.isPlaying)
        {
            audioSourceBG1.Stop();
        }
        player.GetComponent<PlayerMotor>().ResetPlayer();
        player.GetComponent<PlayerHealth>().ResetHealth();
        Debug.Log("Restarting game...");
        OnScreenStick.SetActive(false);
        DestroyInstantiatedRocks();
        ResetUI(); // Reset UI elements
        ResetVariables(); // Reset variables
        ResetGameState(); // Reset game state
        //StartGameplay(); // Start gameplay again
    }

    // Method to reset game state
    private void ResetGameState()
    {
        gameState = GameState.Scanning;
    }

    // Method to reset UI elements
    private void ResetUI()
    {
        Screen.SetActive(false);
        restartButton.SetActive(false);
        diedText.text = "";
        diedScoreText.text = "";
    }

    // Method to reset variables
    private void ResetVariables()
    {
        currentTime = 0f;
        score = 0;
        rockDropCount = 0;
        navMeshBuilt = false;
        knightSpawned = false;
        bishopSpawned = false;
        rookSpawned = false;
        queenSpawned = false;
        kingSpawned = false;

        // Clear lists
        instantiatedGuns.Clear();
        instantiatedHearts.Clear();
        instantiatedRocks.Clear();
    }

    // Method to destroy the rocks that were created
    private void DestroyInstantiatedRocks()
    {
        foreach (GameObject rock in instantiatedRocks)
        {
            Destroy(rock);
        }
        instantiatedRocks.Clear();
    }

    // Drop gun on plane
    private void DropGunOnPlane()
    {
        GameObject randomGun = GetRandomGunPrefab();
        if (randomGun != null && planeScene != null)
        {
            Vector3 randomPosition = GetRandomPositionOnPlane(planeScene);
            Quaternion gunRotation = Quaternion.Euler(-90f, 180f, 0f);
            GameObject instantiatedGun = Instantiate(randomGun, randomPosition, gunRotation);
            
            // Add the instantiated gun to the list
            instantiatedGuns.Add(instantiatedGun);

            // Spawn particles as children of the instantiated gun
            GameObject particlesPrefab = gunSpawnPrefab;
            if (particlesPrefab != null)
            {
                // Offset position for particles
                Vector3 particlesPosition = instantiatedGun.transform.position + new Vector3(0f, 0f, -0.01f);
                GameObject instantiatedParticles = Instantiate(particlesPrefab, particlesPosition, Quaternion.identity, instantiatedGun.transform);
            }

            // Start a coroutine to check if the gun has a parent after 10 seconds
            StartCoroutine(CheckGunParent(instantiatedGun));
        }
    }

    // Drop gun on plane
    private void DropHeartOnPlane()
    {
        if (planeScene != null)
        {
            Vector3 randomPosition = GetRandomPositionOnPlane(planeScene);
            Vector3 heartSpawnPosition = randomPosition;
            GameObject instantiatedHeart = Instantiate(heartPrefab, heartSpawnPosition, heartPrefab.transform.rotation);
            audioSource.PlayOneShot(heartDropSound);
            instantiatedHeart.SetActive(false);
            instantiatedHeart.transform.position += new Vector3(0f, 1f, 0f);
            instantiatedHeart.SetActive(true);
            
            // Add the instantiated gun to the list
            instantiatedHearts.Add(instantiatedHeart);

            StartCoroutine(CheckHeart(instantiatedHeart));
        }
    }
    private IEnumerator CheckHeart(GameObject heart)
    {
        yield return new WaitForSeconds(10f);
        instantiatedHearts.Remove(heart);
        Destroy(heart);
    }

    // Coroutine to check if the gun has a parent after 10 seconds
    private IEnumerator CheckGunParent(GameObject gun)
    {
        yield return new WaitForSeconds(10f);
        // If the gun still doesn't have a parent, destroy it
        if (gun != null && gun.transform.parent == null)
        {
            instantiatedGuns.Remove(gun);
            Destroy(gun);
        }
    }

    // Get a random gun prefab
    private GameObject GetRandomGunPrefab()
    {
        if (weaponPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, weaponPrefabs.Length);
            return weaponPrefabs[randomIndex];
        }
        return null;
    }

    // Coroutine to spawn enemies with delay
    private IEnumerator SpawnEnemiesWithDelay(string enemyPrefabName)
    {
        yield return new WaitForSeconds(5f); // Initial delay before spawning enemies
        for (int i = 0; i < enemyPoolNumber; i++)
            SpawnEnemiesOnPlanes(enemyPrefabName); // Spawn enemies
    }

    // Add this method to your GameManager class
    private void SpawnWeaponForPlayer()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        GameObject prefab = weaponPrefabs[selectedCharacter];
        if (player != null)
        {
            Transform playerTransform = player.transform; // Get the Transform component
            GameObject clone = Instantiate(prefab, playerTransform.position, Quaternion.Euler(-90f, 180f, 0f));
            clone.SetActive(false);
            clone.transform.parent = playerTransform; // Set the player as the parent
            if (prefab.name == "p9")
            {
                clone.transform.localPosition = new Vector3(0.25f, 0.35f, 0f); // Set the local position
            }
            else if (prefab.name == "ar15")
            {
                clone.transform.localPosition = new Vector3(0.125f, 0.3f, 0.05f); // Set the local position
            }
            else if (prefab.name == "xma10")
            {
                clone.transform.localPosition = new Vector3(0.25f, 0.65f, 0.15f); // Set the local position
            }
            clone.SetActive(true);
        }
    }

    // Method to spawn enemies on planes
    private void SpawnEnemiesOnPlanes(string enemyPrefabName)
    {
        if (planeScene == null)
        {
            Debug.LogWarning("Unable to find the largest plane.");
            return;
        }

        Vector3 randomPosition = GetRandomPositionOnPlane(planeScene);
        GameObject enemy = objectPoolManager.GetEnemy(enemyPrefabName);
        if (enemy != null)
        {
            enemy.transform.position = randomPosition;
            enemy.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            enemy.SetActive(true);
        }
    }

    // Find the largest plane in the scene
    private GameObject FindPlane()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Plane")))
            return hit.collider.gameObject;
        return null;
    }

    // Get a random rock prefab
    private GameObject GetRandomRockPrefab()
    {
        if (rockPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, rockPrefabs.Length);
            return rockPrefabs[randomIndex];
        }
        return null;
    }

    // Method to drop rock on plane
    private void DropRockOnPlane()
    {
        GameObject randomRock = GetRandomRockPrefab();
        if (planeScene != null)
        {
            Vector3 randomPosition = GetRandomPositionOnPlane(planeScene);
            Vector3 rockSpawnPosition = randomPosition;
            GameObject instantiatedRock = Instantiate(randomRock, rockSpawnPosition, randomRock.transform.rotation);
            instantiatedRock.SetActive(false);
            instantiatedRock.SetActive(true);

            // Add the instantiated rock to the list
            instantiatedRocks.Add(instantiatedRock);
        }
    }


    // Get random position on a plane
    private Vector3 GetRandomPositionOnPlane(GameObject plane)
    {
        // Get the scale of the plane
        Vector3 planeScale = plane.transform.localScale;

        // Calculate the range for random X and Z coordinates without using rangeMultiplier
        float randomX = Random.Range(plane.transform.position.x - planeScale.x / 2f, plane.transform.position.x + planeScale.x / 2f);
        float randomZ = Random.Range(plane.transform.position.z - planeScale.z / 2f, plane.transform.position.z + planeScale.z / 2f);

        // Keep the Y position the same as the plane's position
        float yPosition = plane.transform.position.y;

        // Return the random position with a slight offset in the Y direction
        return new Vector3(randomX, yPosition, randomZ) + new Vector3(0f, 0.2f, 0f);
    }

    // Coroutine to respawn enemy with delay
    private IEnumerator RespawnEnemyWithDelay(GameObject enemy)
    {
        yield return new WaitForSeconds(2f);

        // Check if the enemy object still exists
        if (enemy != null)
        {
            enemy.transform.position = GetRandomPositionOnPlane(planeScene);
            enemy.SetActive(true);
        }
    }
}