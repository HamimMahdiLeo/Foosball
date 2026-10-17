using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject Pause;
    public GameObject Instruction;

    void Update()
    {
        bool esc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool start = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (esc || start)
        {
            // If the instruction sub-panel is open, close it first
            if (Instruction != null && Instruction.activeSelf)
            {
                CloseInstruction();
            }
            // If pause menu is open, resume the game
            else if (Pause != null && Pause.activeSelf)
            {
                Resume();
            }
            // Otherwise, open pause menu
            else
            {
                OpenPause();
            }
        }
    }

    public void OpenPause()
    {
        Pause.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Pause.SetActive(false);
        if (Instruction != null) Instruction.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenInstruction()
    {
        if (Instruction != null) Instruction.SetActive(true);
    }

    public void CloseInstruction()
    {
        if (Instruction != null) Instruction.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // Unfreeze the game before loading
        SceneManager.LoadScene("Main Menu"); // Redirects to the Main Menu scene
    }
}