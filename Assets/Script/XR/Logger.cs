using System.Linq;
using DilmerGames.Core.Singletons;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Logger : Singleton<Logger>
{
    [SerializeField]
    private TextMeshProUGUI debugAreaText = null;

    [SerializeField]
    public bool enableDebug = false;

    [SerializeField]
    private int maxLines = 15;

    // logger toggle functionality
    private Transform loggerContainer;

    private Button toggleButton;

    private bool isVisible = true;
    public GameObject LoggerContainer;

    void Awake()
    {
        if (debugAreaText != null)
        {
            debugAreaText.text = string.Empty;
        }
        
        // get logger container
        if (transform.childCount > 0)
        {
            loggerContainer = transform.GetChild(0);
        }
        
        toggleButton = GetComponentInChildren<Button>();

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(() =>
            {
                isVisible = !isVisible;
                if(loggerContainer != null)
                    loggerContainer.gameObject.SetActive(isVisible);

                if (toggleButton != null && toggleButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                {
                    toggleButton.GetComponentInChildren<TextMeshProUGUI>()
                        .text = isVisible ? "Hide Log" : "Show Log";
                }
            });
        }
        else
        {
            Debug.LogWarning("Toggle button not found.");
        }
    }

    void OnEnable()
    {
        if (debugAreaText != null)
        {
                
            debugAreaText.enabled = enableDebug;
            enabled = enableDebug;

            if (enabled)
            {
                debugAreaText.text += $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled</color>\n";
            }
        }
    }

    public void toggleDubug()
    {
        if (debugAreaText != null && LoggerContainer != null)
        {
            enableDebug = !enableDebug;
            LoggerContainer.SetActive(enableDebug);
            debugAreaText.enabled = enableDebug;
        }
    }

    public void Clear() => debugAreaText.text = string.Empty;

    public void LogInfo(string message)
    {
        if (debugAreaText != null)
        {
            ClearLines();
            debugAreaText.text += $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        }
    }

    public void LogError(string message)
    {
        if (debugAreaText != null)
        {
            ClearLines();
            debugAreaText.text += $"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        }
    }

    public void LogWarning(string message)
    {
        if (debugAreaText != null)
        {
            ClearLines();
            debugAreaText.text += $"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        }
    }

    private void ClearLines()
    {
        if (debugAreaText != null)
        {
            if (debugAreaText.text.Split('\n').Count() >= maxLines)
            {
                debugAreaText.text = string.Empty;
            }
        }
    }
}