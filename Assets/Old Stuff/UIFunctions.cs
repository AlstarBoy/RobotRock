using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIFunctions : MonoBehaviour
{
    public bool paused;
    public GameObject pauseMenu;

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!paused)
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                paused = true;
            }
            else
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                paused = false;
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        paused = false;
    }


    // Function to load a scene based on the provided scene index.
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

}
