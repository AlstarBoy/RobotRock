using UnityEngine;
using System.Collections;

public class ItemStats : MonoBehaviour
{
    [Header("Stats")]
    public int planetTier;
    public int maxPlanetTier;
    public Vector3[] sizes;       // Array of scales for each tier
    public float[] sizeTime;      // Growth durations per tier
    public bool canGrow = false;
    private placingObject pObject;

    private bool isGrowing = false;
    private bool isAbsorbing = false; // Flag to disable growth during absorption

    [Header("Absorption")]
    public float absorptionSpeed = 0.5f;
    public Transform innerTrigger; // Assign this to a child trigger in the Inspector

    void Start()
    {
        pObject = GetComponent<placingObject>();
    }

    void Update()
    {
        // Only grow if the object is placed, not growing, not absorbing, and hasn't reached the max tier
        if (pObject.isPlaced && !isGrowing && !isAbsorbing && planetTier <= maxPlanetTier)
        {
            canGrow = true;
            StartCoroutine(Grow(sizes[planetTier], sizeTime[planetTier]));
        }
    }

    IEnumerator Grow(Vector3 targetScale, float duration)
    {
        isGrowing = true;
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        // Only grow if the current scale is smaller than the target scale
        if (transform.localScale.x < targetScale.x && transform.localScale.y < targetScale.y && transform.localScale.z < targetScale.z)
        {
            while (elapsedTime < duration)
            {
                // If absorption begins, exit the growth process so it doesn't override absorption changes
                if (isAbsorbing)
                {
                    isGrowing = false;
                    yield break;
                }
                transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        transform.localScale = targetScale;
        isGrowing = false;

        if (planetTier < maxPlanetTier)
        {
            planetTier++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AbsorptionTrigger(other);

        if (other.gameObject.CompareTag("Asteroid"))
        {
            RemoveTier();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        AbsorptionTrigger(other);

        if (other.gameObject.CompareTag("Asteroid"))
        {
            RemoveTier();
        }
    }

    private void AbsorptionTrigger(Collider other)
    {
        if (other.CompareTag("Celestial") && other.GetComponent<placingObject>().isPlaced)
        {
            ItemStats otherPlanet = other.GetComponent<ItemStats>();
            if (otherPlanet != null && otherPlanet != this)
            {
                // Check if the other planet is smaller
                if (otherPlanet.transform.localScale.magnitude < transform.localScale.magnitude)
                {
                    StartCoroutine(AbsorbPlanet(otherPlanet));
                }
            }
        }
    }

    IEnumerator AbsorbPlanet(ItemStats targetPlanet)
    {
        if (targetPlanet == null)
            yield break;

        // Disable growth while absorbing
        isAbsorbing = true;

        // Store the target's original scale before any shrinking occurs
        Vector3 originalTargetScale = targetPlanet.transform.localScale;

        while (targetPlanet != null && targetPlanet.transform.localScale.magnitude > 0.1f)
        {
            // Move the smaller planet toward the bigger one
            targetPlanet.transform.position = Vector3.MoveTowards(targetPlanet.transform.position, transform.position, absorptionSpeed * Time.deltaTime);

            // Gradually shrink the smaller planet
            targetPlanet.transform.localScale *= (1 - Time.deltaTime * absorptionSpeed);

            // Check if it reaches the inner trigger
            if (Vector3.Distance(targetPlanet.transform.position, innerTrigger.position) < 0.1f)
            {
                // Transfer a portion (50%) of the original scale of the target to this planet
                transform.localScale += originalTargetScale * 0.5f;

                // Destroy the absorbed planet
                Destroy(targetPlanet.gameObject);
                break;
            }
            yield return null;
        }

        // Re-enable growth after absorption
        isAbsorbing = false;
    }

    private void RemoveTier()
    {
        // Only remove a tier if the current tier is above 0
        if (planetTier > 0)
        {
            planetTier--;
            // Adjust the scale to match the previous tier's scale from the sizes array.
            transform.localScale = sizes[planetTier];
            Debug.Log("Planet tier removed. New tier: " + planetTier);
        }
        else
        {
            Debug.Log("No lower tier to remove.");
        }
    }
}
