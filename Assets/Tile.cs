using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public GameObject gOccupier;
    public bool occupied;
    public Vector3 myPos;

    void Awake()
    {
        this.name = $"Tile {this.transform.position.x} {this.transform.position.y} {this.transform.position.z}";
    }

    private void Update()
    {
        if (gOccupier != null)
        {
            occupied = true;
        }
        else
        {
            occupied=false;
            gOccupier = null;
        }
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
