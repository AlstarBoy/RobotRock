using UnityEngine;
using TMPro; // Required for TextMeshPro

/// <summary>
/// Manages player turns and updates TMP UI for moves and score.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Game Settings")]
    public int startingMoves = 10;
    public int remainingMoves;

    [Header("UI Elements (TextMeshPro)")]
    public TMP_Text movesTMPText;
    public TMP_Text scoreTMPText;

    private int currentScore;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        remainingMoves = startingMoves;
        UpdateUI();
    }

    /// <summary>
    /// Call this after each move. Decreases moves and updates UI.
    /// </summary>
    public void MakeMove()
    {
        if (remainingMoves <= 0) return;

        remainingMoves--;
        UpdateUI();

        if (remainingMoves <= 0)
        {
            Debug.Log("Out of moves! Level over.");
            GameOver();
        }
    }

    /// <summary>
    /// Adds points to the current score.
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    /// <summary>
    /// Updates all TMP UI elements.
    /// </summary>
    private void UpdateUI()
    {
        if (movesTMPText != null)
            movesTMPText.text = $"Moves: {remainingMoves}";

        if (scoreTMPText != null)
            scoreTMPText.text = $"Score: {currentScore}";
    }

    private void GameOver()
    {
        // TODO: Disable input, show Game Over UI, etc.
        Debug.Log("Game Over. Final Score: " + currentScore);
    }

    public int GetScore()
    {
        return currentScore;
    }
}

