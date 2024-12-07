using UnityEngine;
using TMPro;

public class ComboCounter : MonoBehaviour
{
    public int currentCombo;
    public int highestCombo;

    public TextMeshProUGUI comboText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        comboText.text = "x" + currentCombo;

        if (currentCombo > highestCombo)
        {
            highestCombo = currentCombo;
        }
    }
}
