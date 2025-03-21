using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public float score = 0;
    public TextMeshProUGUI scoreUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreUI.text = "Score: " + score;
    }

    public void IncreaseScore(float score)
    { 
        this.score += score; 
    }
}
