using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform spawnPoint;
    public Material[] materials;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        int selectedMaterial = PlayerPrefs.GetInt("selectedMaterial");
        GameObject prefab = characterPrefabs[selectedCharacter];
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Destroy(spawnPoint.gameObject); // Destroy the spawnPoint GameObject
        
        clone.SetActive(false);
        
        Renderer renderer = clone.GetComponentInChildren<Renderer>();
        renderer.material = materials[selectedMaterial];
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            clone.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            clone.transform.parent = playerObject.transform;
            clone.SetActive(true);
        }
        else
        {
            Debug.LogError("No object with the tag 'Player' found!");
        }
    }

    void Update()
    {
        
    }
}
