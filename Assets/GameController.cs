using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Narrative Settings")]
    public Image[] narrativeImages; // Images for the narrative.
    private int currentNarrativeIndex = 0;
    private bool narrativeActive = true;
    private float narrativeTimer = 0f;
    private float narrativeDisplayTime = 5f; // Time in seconds for each narrative image.

    [Header("Timer Settings")]
    public TextMeshProUGUI timerText; // UI text to display the timer.
    public TextMeshProUGUI timerTextBACK; // UI text to display the timer.
    private float survivalTime;
    private bool gameActive = false;

    [Header("Enemy Settings")]
    public GameObject enemyType1;
    public GameObject enemyType2;
    public Transform[] spawnPoints; // Array of spawn points.
    public float initialSpawnRate = 2f; // Initial spawn rate in seconds.
    private float spawnRate;
    private float spawnTimer;

    [Header("UI Settings")]
    public GameObject loseScreen;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI finalTimeTextBACK;
    public TextMeshProUGUI maxComboTextBACK;

    [Header("Pause Settings")]
    public GameObject pauseMenu;
    private bool isPaused = false;
    private bool gameOver;

    private void Start()
    {
        // Initialize variables.
        spawnRate = initialSpawnRate;
        loseScreen.SetActive(false);
        pauseMenu.SetActive(false);
        ShowNextNarrativeImage();
        gameOver = false;
    }

    private void Update()
    {
        if (narrativeActive)
        {
            // Automatically progress the narrative images after the display time.
            narrativeTimer += Time.deltaTime;
            if (narrativeTimer >= narrativeDisplayTime || Input.GetKeyDown(KeyCode.Escape))
            {
                narrativeTimer = 0f;
                ShowNextNarrativeImage();
            }
        }
        else if (gameActive)
        {
            // Timer update.
            survivalTime += Time.deltaTime;
            timerText.text = "Time: " + survivalTime.ToString("F2") + "s";
            timerTextBACK.text = "Time: " + survivalTime.ToString("F2") + "s";

            // Enemy spawning.
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnRate)
            {
                SpawnEnemy();
                spawnTimer = 0;
                spawnRate = Mathf.Max(0.5f, spawnRate * 0.98f); // Gradually increase spawn rate.
            }
            // Pause functionality.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }


    }

    // Narrative progression.
    public void ShowNextNarrativeImage()
    {
        if (currentNarrativeIndex < narrativeImages.Length)
        {
            narrativeImages[currentNarrativeIndex].gameObject.SetActive(true);
            if (currentNarrativeIndex > 0)
                narrativeImages[currentNarrativeIndex - 1].gameObject.SetActive(false);

            currentNarrativeIndex++;
        }
        else
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        foreach (Image img in narrativeImages)
        {
            img.gameObject.SetActive(false);
        }
        narrativeActive = false;
        gameActive = true;
    }

    // Spawn enemies at random spawn points.
    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = Random.Range(0, 2) == 0 ? enemyType1 : enemyType2;
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    // Trigger the lose screen.
    public void GameOver(int maxCombo)
    {
        gameOver = true;
        gameActive = false;
        loseScreen.SetActive(true);
        finalTimeText.text = "" + survivalTime.ToString("F2") + "s";
        maxComboText.text = "" + maxCombo;
        finalTimeTextBACK.text = "" + survivalTime.ToString("F2") + "s";
        maxComboTextBACK.text = "" + maxCombo;
    }

    // Pause functionality.
    public void TogglePause()
    {
        if (!gameOver)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
        }
        else if (gameOver)
        {
            RestartGame();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Factory");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


