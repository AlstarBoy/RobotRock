using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
    public ScoreSystem scoreSystem;
    [Header("Singularity")]
    public int maxSingularityOverload = 100;
    public int currentSingularityOverload;
    public int totalTiers;
    public int totalCelestial;
    public RectTransform singularityFill;

    [Header("SingularityEvents")]
    public bool[] sEvents;
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





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
            gameOverScreen.SetActive(true);
        }

    }

    void SingularityEvent()
    {
        currentSingularityOverload = totalTiers + totalCelestial;
        float x = -92f + (((float)currentSingularityOverload / maxSingularityOverload) * 92f);
        Vector2 pos = singularityFill.anchoredPosition;
        pos.x = x;
        singularityFill.anchoredPosition = pos;
        if (currentSingularityOverload > maxSingularityOverload)
        {

        }
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
        if (context.performed)
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
