using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
    [Header("Timer")]
    public TextMeshProUGUI timerUI;
    public float myTime;
    [Header("UI")]
    public TextMeshProUGUI levelUI;
    public TextMeshProUGUI singularity;
    public TextMeshProUGUI annoucement;

    [Header("Level")]
    public int currentLevel;
    public int[] levelUpThresholds;
    public float[] gameSpeed;
    public float currentGameSpeed;
    public GameObject GameOverUI;
    public float celestialTotal;
    public int maxSingularityOverload;
    public int currentSingularityOverload;
    public ScoreSystem scoreSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (nextObject == null)
        {
            nextObject = celestialObjects[Random.Range(0, celestialObjects.Length)];
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

        if (scoreSystem.score > levelUpThresholds[currentLevel])
        {
            IncreaseLevel();
        }
    }

    public void countCelestials()
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

                int randInt = Random.Range(0, celestialObjects.Length);
                nextObject = celestialObjects[randInt];
            }
            currentObject = Instantiate(currentObject, startPos.transform.position, Quaternion.identity);
            currentObject.GetComponent<placingObject>().mousePos = startPos;
            currentObject.GetComponent<placingObject>().gameC = this;
            objectPlaced = false;
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
