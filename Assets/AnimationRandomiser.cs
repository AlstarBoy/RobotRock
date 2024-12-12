using System;
using UnityEngine;

public class AnimationRandomiser : MonoBehaviour
{
    public int comboNumber;
    public int randomNumber;

    void RandomiseAnimation(int comboNumber, float distance, int randNum)
    {
        if (distance < 3)
        {   
            if (comboNumber == 1)
            {
                randNum = UnityEngine.Random.Range(0, 5);
            }
            else if (comboNumber == 2)
            {
                randNum = UnityEngine.Random.Range(0, 6);
            }
            else if (comboNumber == 3)
            {
                randNum = UnityEngine.Random.Range(0, 4);
            }
        }
        else if (distance > 3)
        {
            if (comboNumber == 1)
            {
                randNum = UnityEngine.Random.Range(0, 5);
            }
            else if (comboNumber == 2)
            {
                randNum = UnityEngine.Random.Range(0, 5);
            }
            else if (comboNumber == 3)
            {
                randNum = UnityEngine.Random.Range(0, 5);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        comboNumber = UnityEngine.Random.Range(0, 3);
    }
}
