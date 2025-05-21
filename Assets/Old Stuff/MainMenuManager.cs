using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Typewriter Settings")]
    public TextMeshProUGUI tmpText;          // Reference to the TextMeshProUGUI component
    [TextArea]
    public string fullText;                  // The complete text to display
    public float delayBetweenLetters = 0.1f;   // Delay between each letter

    [Header("Movement Settings")]
    // The RectTransform of the UI element that will be moved after the text is complete.
    public RectTransform movingTextRectTransform;
    // The target UI element's RectTransform.
    public RectTransform targetRectTransform;
    public float moveSpeed = 5f;             // Speed at which the text will move

    private bool isTextCompleted = false;

    [Header("Menu Buttons")]
    public GameObject menuButtons;

    [Header("Fade")]
    public RawImage fadeImage;

    void Start()
    {
        Time.timeScale = 1.0f;

        if (tmpText != null)
        {
            tmpText.text = "";
            StartCoroutine(TypeText());
        }
        else
        {
            Debug.LogWarning("TMP Text component is not assigned.");
        }
    }

    // Coroutine to display text one letter at a time.
    IEnumerator TypeText()
    {
        foreach (char letter in fullText.ToCharArray())
        {
            tmpText.text += letter;
            yield return new WaitForSeconds(delayBetweenLetters);
        }
        isTextCompleted = true;
    }

    void Update()
    {
        // After the text is fully displayed, move the movingTextRectTransform toward the target's position.
        if (isTextCompleted && movingTextRectTransform != null && targetRectTransform != null)
        {
            float step = moveSpeed * Time.deltaTime;
            movingTextRectTransform.anchoredPosition = Vector2.MoveTowards(
                movingTextRectTransform.anchoredPosition,
                targetRectTransform.anchoredPosition,
                step);
        }

        if (movingTextRectTransform.anchoredPosition == targetRectTransform.anchoredPosition)
        {
            menuButtons.SetActive(true);
        }
    }

    // Function to load a scene based on the provided scene index.
    public void LoadSceneByIndex(int sceneIndex)
    {
        StartCoroutine(FadeOut(sceneIndex));
        StartCoroutine(FadeOutVolume());
    }
    public IEnumerator FadeOut(int scene)
    {
        float timer = 0f;
        Color col = fadeImage.color;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            col.a = Mathf.Lerp(0f, 1f, timer / 1);
            fadeImage.color = col;
            yield return null;
        }
        col.a = 1f;
        fadeImage.color = col;
        SceneManager.LoadScene(scene);
    }

    public AudioSource audioSource; // The AudioSource whose volume will be faded.
    public float audioFadeDuration = 1f; // Duration over which the volume will fade to zero.
    private IEnumerator FadeOutVolume()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;
        while (timer < audioFadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / audioFadeDuration);
            yield return null;
        }
        audioSource.volume = 0f; // Ensure volume is exactly zero.
    }

    // Function to close the application.
    public void CloseApplication()
    {
        StartCoroutine(FadeOutQuit());
        StartCoroutine(FadeOutVolume());
    }

    public IEnumerator FadeOutQuit()
    {
        float timer = 0f;
        Color col = fadeImage.color;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            col.a = Mathf.Lerp(0f, 1f, timer / 1);
            fadeImage.color = col;
            yield return null;
        }
        col.a = 1f;
        fadeImage.color = col;
        Application.Quit();
    }
}

