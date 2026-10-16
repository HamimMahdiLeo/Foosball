using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Required for the new Input System

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject instructionsPanel; // Drag your InstructionsPanel here in Unity

    [Header("Links")]
    public string googleFormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSdmMirgKKuhcSzAfCFQSaVZr0__OdTUfwHRQ11C2PhRYlSiTw/viewform";

    void Update()
    {
        // Check if the Escape key is pressed using the new Input System
        if (instructionsPanel != null && instructionsPanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInstruction();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenInstruction()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true); // Show the instructions panel
        }
    }

    public void CloseInstruction()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false); // Hide the instructions panel
        }
    }

    public void OpenFeedback()
    {
        Application.OpenURL(googleFormUrl);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}