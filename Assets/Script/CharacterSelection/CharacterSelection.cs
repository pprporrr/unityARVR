using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public int selectedCharacter = 0;
    public Material[] materials;
    public Button whiteButton;
    public Button blackButton;

    private int currentMaterialIndex = 0;

    void Start()
    {
        UpdateCharacterMaterials();
    }

    void UpdateCharacterMaterials()
    {
        foreach (GameObject character in characters)
        {
            Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (materials.Length > 0)
                {
                    renderer.material = materials[currentMaterialIndex];
                }
            }
        }
    }

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
        UpdateCharacterMaterials();
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
        UpdateCharacterMaterials();
    }

    public void ToggleMaterial()
    {
        currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length;
        UpdateCharacterMaterials();
        whiteButton.gameObject.SetActive(!whiteButton.gameObject.activeSelf);
        blackButton.gameObject.SetActive(!blackButton.gameObject.activeSelf);
    }

    public void StartGame()
    {
        try
        {
            SceneManager.UnloadSceneAsync(1);
        }
        catch (System.Exception)
        {
            PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
            PlayerPrefs.SetInt("selectedMaterial", currentMaterialIndex);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}