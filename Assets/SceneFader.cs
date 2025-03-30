using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFaderRawImage : MonoBehaviour
{
    [Header("Fade Settings")]
    // Reference to the RawImage covering the screen (should be black).
    public RawImage fadeImage;
    // Duration of the fade effect in seconds.
    public float fadeDuration = 1f;

    void Awake()
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image is not assigned!");
            return;
        }
        // Ensure the image starts fully opaque (black).
        Color col = fadeImage.color;
        col.a = 1f;
        fadeImage.color = col;

        // Automatically fade away (fade in to reveal the scene) on Awake.
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fades from opaque black to clear.
    /// </summary>
    public IEnumerator FadeIn()
    {
        float timer = 0f;
        Color col = fadeImage.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            col.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = col;
            yield return null;
        }
        col.a = 0f;
        fadeImage.color = col;
    }

    /// <summary>
    /// Fades from clear to opaque black.
    /// </summary>
    public IEnumerator FadeOut()
    {
        float timer = 0f;
        Color col = fadeImage.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            col.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = col;
            yield return null;
        }
        col.a = 1f;
        fadeImage.color = col;
    }

    /// <summary>
    /// Public method to start a fade in (black -> clear).
    /// </summary>
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Public method to start a fade out (clear -> black).
    /// </summary>
    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }
}