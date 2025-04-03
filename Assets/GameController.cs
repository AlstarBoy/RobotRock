using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class GameController : MonoBehaviour
{
    [Header("Random Object Array")]
    public GameObject[] celestialObjects;
    [Header("Current/Next Object")]
    public GameObject nextObject;
    public GameObject currentObject;
    public GameObject startPos;
    public GameObject nextUI;
    public bool objectPlaced = false;
    public GameObject[] nextObjectIcon;
    public int nextONum;
    [Header("Re-Roll Next Object")]
    public int lastObject = -1;
    public int lastObjectCounter;
    [Header("Timer")]
    public TextMeshProUGUI timerUI;
    public float myTime;
    [Header("UI")]
    public TextMeshProUGUI levelUI;
    public TextMeshProUGUI singularity;
    public TextMeshProUGUI annoucement;
    public TextMeshProUGUI celestialsUI;

    [Header("Level")]
    public int currentLevel;
    public int[] levelUpThresholds;
    public float[] gameSpeed;
    public float currentGameSpeed;
    public GameObject GameOverUI;
    public TextMeshProUGUI highScore;
    public ScoreSystem scoreSystem;
    [Header("Singularity")]
    public int maxSingularityOverload = 100;
    public int currentSingularityOverload;
    public int totalTiers;
    public int totalCelestial;
    public int totalCelestialO;
    public RectTransform singularityFill;

    [Header("SingularityEvents")]
    public bool[] sEvents;
    public bool startEvent = false;
    public bool eventRunning = false;
    [Header("Temporal Distortions")]
    public bool temporalEvent = false;
    public GameObject[] timeZones;
    

    [Header("Balance Scale")]
    public int currentBadCelestials;
    public int currentGoodCelestials;
    public int maxBadCelestials = -100;
    public int maxGoodCelestials = 100;
    public int currentGoodBadCelestial = 0;
    public RectTransform balanceScalePoint;
    public RectTransform scaleBar;
    [Header("Balance Scale")]
    public GameObject gameOverScreen;

    [Header("Tiles")]
    public GameObject tileHolder;
    public GameObject[] tiles;

    void Awake()
    {
        Time.timeScale = 1.0f;
        // Ensure tileHolder is assigned to prevent NullReferenceException
        if (tileHolder == null)
        {
            Debug.LogError("tileHolder is not assigned in the inspector.");
            return;
        }

        // Populate the tiles array with all direct children of tileHolder
        int childCount = tileHolder.transform.childCount;
        tiles = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            tiles[i] = tileHolder.transform.GetChild(i).gameObject;
        }

        if (nextObject == null)
        {
            int randInt = Random.Range(0, celestialObjects.Length);
            nextObject = celestialObjects[randInt];
            nextONum = randInt;
            nextObjectIcon[nextONum].SetActive(true);
            lastObject = randInt;
        }
        pickObject();
        currentGameSpeed = gameSpeed[currentLevel];

    }



    // Update is called once per frame
    void Update()
    {
        if (objectPlaced)
        {
            pickObject();
        }

        GlobalTimer();
        levelUI.text = "level: " + currentLevel;
        celestialsUI.text = "celestials:" + totalCelestial;
        SingularityEvent();
        BalanceScale();

        if (scoreSystem.score > levelUpThresholds[currentLevel])
        {
            IncreaseLevel();
        }
    }

    void BalanceScale()
    {
        // Assuming bad celestials are negative numbers,
        // compute the net value by adding (not subtracting)
        currentGoodBadCelestial = currentGoodCelestials + currentBadCelestials;

        // Remap net value from [maxBadCelestials, maxGoodCelestials] to [-85, 85]
        float x = -85f + (((float)currentGoodBadCelestial - maxBadCelestials) / (maxGoodCelestials - maxBadCelestials)) * 170f;

        // Update the UI RectTransform's anchoredPosition (since you can’t change just x directly)
        Vector2 pos = balanceScalePoint.anchoredPosition;
        pos.x = x;
        balanceScalePoint.anchoredPosition = pos;

        // ROTATION
        currentGoodBadCelestial = currentGoodCelestials + currentBadCelestials;
        float zRot = 20f + (((float)currentGoodBadCelestial - maxBadCelestials) / (maxGoodCelestials - maxBadCelestials)) * -40f;
        scaleBar.rotation = Quaternion.Euler(0, 0, zRot);

        if (currentGoodBadCelestial <= maxBadCelestials || currentGoodBadCelestial >= maxGoodCelestials)
        {
            Time.timeScale = 0f;
            highScore.text = "high score: " + scoreSystem.score;
            gameOverScreen.SetActive(true);
        }
    }

    void SingularityEvent()
    {
        currentSingularityOverload = totalTiers + totalCelestialO;
        float x = -92f + (((float)currentSingularityOverload / maxSingularityOverload) * 92f);
        Vector2 pos = singularityFill.anchoredPosition;
        pos.x = x;
        singularityFill.anchoredPosition = pos;
        if (currentSingularityOverload >= maxSingularityOverload)
        {
            currentSingularityOverload = maxSingularityOverload;
            startEvent = true;
            if (startEvent && !eventRunning)
            {
                StartCoroutine(TemporalDistortion());
            }
        }
    }

    IEnumerator TemporalDistortion()
    {
        eventRunning = true;
        temporalEvent = true;

        // Build a list of available positions from the 'tiles' array.
        List<Vector3> availablePositions = new List<Vector3>();
        foreach (GameObject posHolder in tiles)
        {
            availablePositions.Add(posHolder.transform.position);
        }

        // Warn if there are fewer available positions than timeZones.
        if (availablePositions.Count < timeZones.Length)
        {
            Debug.LogWarning("There are fewer available positions than timeZones. Some objects may not be placed.");
        }

        // Place each timeZone one by one.
        for (int i = 0; i < timeZones.Length; i++)
        {
            if (availablePositions.Count == 0)
            {
                Debug.LogWarning("No available positions remain for timeZone index " + i);
                break;
            }

            bool placed = false;
            // Attempt to find a valid (non-overlapping) position for the current timeZone.
            while (!placed && availablePositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                Vector3 candidatePosition = availablePositions[randomIndex];

                // Randomize scale as a whole number between 1 and 7.
                int randomScale = Random.Range(1, 8); // 1 to 7 (8 is exclusive)

                // Tentatively place the object.
                timeZones[i].transform.position = candidatePosition;
                timeZones[i].transform.localScale = Vector3.one * randomScale;
                timeZones[i].SetActive(true);

                // Determine a radius for the overlap check. (Assuming uniform scale,
                // using half the scale as an approximate radius.)
                float checkRadius = timeZones[i].transform.localScale.x * 0.5f;

                // Perform an overlap sphere check at the candidate position.
                Collider[] overlaps = Physics.OverlapSphere(candidatePosition, checkRadius);

                bool overlapFound = false;
                foreach (Collider col in overlaps)
                {
                    // Check for other objects with the tag "TemporalDistortions".
                    if (col.gameObject != timeZones[i] && col.CompareTag("TemporalDistortions"))
                    {
                        overlapFound = true;
                        break;
                    }
                }

                if (!overlapFound)
                {
                    // Candidate is valid. Remove this position from the list.
                    availablePositions.RemoveAt(randomIndex);
                    placed = true;
                }
                else
                {
                    // Overlap detected. Remove this candidate so it is not reused.
                    availablePositions.RemoveAt(randomIndex);
                    // Deactivate the object since this candidate failed.
                    timeZones[i].SetActive(false);
                }
            }

            if (!placed)
            {
                Debug.LogWarning("Could not find a valid placement for timeZone index " + i);
            }
        }

        // Wait for 30 seconds.
        yield return new WaitForSeconds(30f);

        // After waiting, deactivate all timeZones.
        for (int i = 0; i < timeZones.Length; i++)
        {
            timeZones[i].SetActive(false);
        }

        eventRunning = false;
        temporalEvent = false;
        currentSingularityOverload = 0;
        totalCelestialO = 0;
        totalTiers = 0;
    }

    void QuantumShift()
    {

    }

    void DarkMatterSurge()
    {

    }

    void IncreaseLevel()
    {
        currentLevel += 1;
        currentGameSpeed = gameSpeed[currentLevel];
    }

    void pickObject()
    {
        if (currentObject == null || objectPlaced == true)
        {
            currentObject = nextObject;
            if (nextObject == null || objectPlaced == true)
            {
                nextObjectIcon[nextONum].SetActive(false);
                int randInt = Random.Range(0, celestialObjects.Length);
                reRollPickObject(randInt);
                nextObject = celestialObjects[randInt];
                nextONum = randInt;
                nextObjectIcon[nextONum].SetActive(true);
                lastObject = randInt;
            }
            currentObject = Instantiate(currentObject, startPos.transform.position, Quaternion.identity);
            currentObject.GetComponent<placingObject>().mousePos = startPos;
            currentObject.GetComponent<placingObject>().gameC = this;
            objectPlaced = false;
        }
    }

    void reRollPickObject(int currentObject)
    {

        if (currentObject == lastObject)
        {
            lastObjectCounter++;
            for (int i = 0; i < lastObjectCounter; i++)
            {
                currentObject = Random.Range(0, celestialObjects.Length);
                print("Reroll");
            }
        }
        else
        {
            lastObjectCounter = 0;
        }
    }

    public void quickPlaceObject(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            currentObject.GetComponent<placingObject>().placeObjectSmoothly();
        }
    }

    void GlobalTimer()
    {
        myTime = Time.fixedTime;
        // get the total full seconds.
        var t0 = (int)myTime;

        // get the number of minutes.
        var m = t0 / 60;

        // get the remaining seconds.
        var s = (t0 - m * 60);

        // get the 2 most significant values of the milliseconds.
        var ms = (int)((myTime - t0) * 100);

        timerUI.text = $"{m:00}:{s:00}:{ms:00}";
    }
}
