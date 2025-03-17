using UnityEngine;
using System.Collections;

public class ItemStats : MonoBehaviour
{
    [Header("Stats")]
    public int planetTier;
    public int maxPlanetTier;
    public Vector3[] sizes;
    public float[] sizeTime;
    public bool canGrow = false;
    private placingObject pObject;

    private bool isGrowing = false;

    void Start()
    {
        pObject = GetComponent<placingObject>();
    }

    void Update()
    {
        if (pObject.isPlaced && !isGrowing && planetTier <= maxPlanetTier)
        {
            canGrow = true;
            print("placed");
            StartCoroutine(Grow(sizes[planetTier], sizeTime[planetTier]));
        }
    }

    IEnumerator Grow(Vector3 targetScale, float duration)
    {
        isGrowing = true; // Prevent multiple coroutine starts
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it reaches exact size
        isGrowing = false;

        if (planetTier < maxPlanetTier)
        {
            planetTier++; // Increase the tier only after growth is complete
        }
    }
}

