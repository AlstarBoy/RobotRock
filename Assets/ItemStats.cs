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

    [Header("Absorption")]
    public float absorptionSpeed = 0.5f;
    public Transform innerTrigger; // Assign this to the child trigger in the Inspector

    void Start()
    {
        pObject = GetComponent<placingObject>();
    }

    void Update()
    {
        if (pObject.isPlaced && !isGrowing && planetTier <= maxPlanetTier)
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

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
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
        ItemStats otherPlanet = other.GetComponent<ItemStats>();
        if (otherPlanet != null && otherPlanet != this)
        {
            if (otherPlanet.transform.localScale.magnitude < transform.localScale.magnitude) // Is smaller
            {
                StartCoroutine(AbsorbPlanet(otherPlanet));
            }
        }
    }

    IEnumerator AbsorbPlanet(ItemStats targetPlanet)
    {
        print("Absorb Planet Start");
        while (targetPlanet != null && targetPlanet.transform.localScale.magnitude > 0.1f)
        {
            print("Can Absorb Planet");
            // Move the smaller planet towards the bigger one
            targetPlanet.transform.position = Vector3.MoveTowards(targetPlanet.transform.position, transform.position, absorptionSpeed * Time.deltaTime);

            // Reduce size of the smaller planet
            targetPlanet.transform.localScale = Vector3.Lerp(targetPlanet.transform.localScale, Vector3.zero, Time.deltaTime * absorptionSpeed);

            // Check if it reaches the inner trigger
            if (Vector3.Distance(targetPlanet.transform.position, innerTrigger.position) < 0.1f)
            {
                print("Inner Ring hit");
                // Absorb remaining scale
                transform.localScale += targetPlanet.transform.localScale * 0.5f;

                // Destroy the absorbed planet
                Destroy(targetPlanet.gameObject);
                yield break;
            }

            yield return null;
        }
    }
}