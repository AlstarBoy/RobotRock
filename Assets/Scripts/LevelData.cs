using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    /// <summary>
    /// Represents the data for a single level.
    /// </summary>

    public int levelID;
    public int moveLimit;
    public int targetScore;
    public GameObject[] fusionObjectives;
    public GameObject[] fusionObjectivesNum;
    public string levelName;
    public string description;
    public string backgroundTheme;
    public string musicTrack;
}
