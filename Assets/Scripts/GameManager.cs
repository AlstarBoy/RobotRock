using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Object Array")]
    public GameObject[] celestialObjects;

    [Header("Current/Next Object")]
    public GameObject nextObject;
    public GameObject currentObject;
    public GameObject startPos;
    public bool canPlace = false;

    [Header("UI")]
    public TextMeshProUGUI levelUI;
    public TextMeshProUGUI singularity;
    public TextMeshProUGUI annoucement;
    public TextMeshProUGUI celestialsUI;

    [Header("Level")]
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

    public 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // check if merges can happen
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
