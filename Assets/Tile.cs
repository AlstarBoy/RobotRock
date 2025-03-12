using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    void Awake()
    {
        this.name = $"Tile {this.transform.position.x} {this.transform.position.y} {this.transform.position.z}";
    }

    /*
    void OnMouseEnter()
    {
        
        if (_highlight != null)
        {
            _highlight.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (_highlight != null)
        {
            _highlight.SetActive(false);
        }
    }
    */
}
