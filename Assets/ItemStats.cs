using UnityEngine;
using System.Collections;
using System.Threading;

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
    private bool isRemoveTier = false;
    public bool isBlackHole = false;

    public bool allowedToGrow;
    public bool allowedToAbsorb;
    public bool allowedToRemoveTier;

    [Header("Absorption")]
    public float absorptionSpeed = 0.5f;
    public Transform innerTrigger; // Assign this to a child trigger in the Inspector

    [Header("Score")]
    private ScoreSystem scoreSystem;
    public int scoreWhenTierUp;
    public int scoreWhenAbsorb;



    void Awake()
    {
        // Find the score system on Awake
        if (scoreSystem == null)
        {
            scoreSystem = GameObject.Find("=== Score System").GetComponent<ScoreSystem>();
        }
        pObject = GetComponent<placingObject>();
    }

    void Update()
    {
        CheckCollision();
        // Only grow if:
        // - The object is placed,
        // - Not currently growing or absorbing,
        // - There's a next tier available,
        // - And growth is allowed.
        if (pObject.isPlaced && !isGrowing && !isAbsorbing && planetTier < maxPlanetTier && allowedToGrow)
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

        // Only proceed if the current scale is smaller than the target scale.
        if (transform.localScale.x < targetScale.x &&
            transform.localScale.y < targetScale.y &&
            transform.localScale.z < targetScale.z)
        {
            while (elapsedTime < duration)
            {
                // If absorption or tier removal begins, abort growth.
                if (isAbsorbing || isRemoveTier)
                {
                    isGrowing = false;
                    yield break;
                }
                transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Ensure we end exactly at the target scale.
        transform.localScale = targetScale;
        isGrowing = false;

        // Tier up after growth if there's room.
        if (planetTier < maxPlanetTier)
        {
            scoreSystem.IncreaseScore(scoreWhenTierUp * (planetTier + 1));
            planetTier++;
        }
    }

    // We use OnTriggerEnter only to avoid repeated calls.
    private void OnTriggerEnter(Collider other)
    {
        // Handle absorption for both asteroids (if allowed) and Celestial objects.
        AbsorptionTrigger(other);
    }

    private void AbsorptionTrigger(Collider other)
    {
        if ((other.CompareTag("Asteroid") && other.GetComponent<placingObject>().isPlaced && allowedToAbsorb) ||
            (other.CompareTag("Celestial") && other.GetComponent<placingObject>().isPlaced && allowedToAbsorb))
        {
            ItemStats otherPlanet = other.GetComponent<ItemStats>();
            if (otherPlanet != null && otherPlanet != this)
            {
                // Only absorb if the other planet is smaller.
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

        // Disable growth while absorption is active.
        isAbsorbing = true;

        while (targetPlanet != null && targetPlanet.transform.localScale.magnitude > 0.1f && !targetPlanet.isBlackHole
            || targetPlanet != null && targetPlanet.transform.localScale.magnitude > 0.1f && targetPlanet.isBlackHole && isBlackHole 
            || targetPlanet != null && !targetPlanet.isBlackHole && isBlackHole)
        {
            // Move the target toward this planet.
            targetPlanet.transform.position = Vector3.MoveTowards(targetPlanet.transform.position, transform.position, absorptionSpeed * Time.deltaTime);

            // Gradually shrink the target.
            targetPlanet.transform.localScale *= (1 - Time.deltaTime * absorptionSpeed);

            // When the target reaches the inner trigger zone, perform the absorption.
            if (Vector3.Distance(targetPlanet.transform.position, innerTrigger.position) < 0.1f)
            {
                // If colliding with an asteroid and removal is allowed, remove a tier.
                if (targetPlanet.gameObject.CompareTag("Asteroid") && allowedToRemoveTier)
                {
                    RemoveTier();
                }
                // Instead of adding a fixed vector, we now check if we can go to the next tier.
                else if (planetTier < maxPlanetTier) // There is a next tier available
                {
                    print("absorb planet size");
                    // Snap the absorber's scale to the next defined size.
                    transform.localScale = sizes[planetTier];
                    scoreSystem.IncreaseScore(scoreWhenAbsorb * (planetTier + 1));
                }
                else
                {
                    print("absorb planet score");
                    // If at max tier, just add score.
                    scoreSystem.IncreaseScore(scoreWhenAbsorb * (planetTier + 1));
                }
                //yield on a new YieldInstruction that waits for 5 seconds.
                yield return new WaitForSeconds(0.1f);
                // Destroy the absorbed planet.
                Destroy(targetPlanet.gameObject);
                break;
            }
            yield return null;
        }

        // Re-enable growth after absorption.
        isAbsorbing = false;
    }

    private void RemoveTier()
    {
        // Only remove a tier if current tier is above 0.
        if (planetTier > 0)
        {
            isRemoveTier = true;
            planetTier--;
            // Snap scale to the previous tier's size.
            transform.localScale = sizes[planetTier-1];
            isRemoveTier = false;
        }
    }




    [Header("CheckCollision")]
    public SphereCollider sCol;
    public BoxCollider bCol;
    public float updateTime;
    public float cTime;

    public void CheckCollision()
    {
        if (!isAbsorbing)
        {
            if (cTime < updateTime)
            {
                bCol.enabled = true;
                sCol.enabled = true;
                cTime += Time.fixedDeltaTime;
            }
            else
            {
                bCol.enabled = false;
                sCol.enabled = false;
                cTime = 0;
            }
        }
        else
        {
            bCol.enabled = true;
            sCol.enabled = true;
        }

    }



}

