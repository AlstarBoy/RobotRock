
using UnityEngine;

/// <summary>
/// Represents a tile in the grid.
/// </summary>
public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public CelestialObject currentObject;

    public bool IsOccupied => currentObject != null;

    private void Awake()
    {
        gridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
        this.name = $"Tile {this.transform.position.x} {this.transform.position.y} {this.transform.position.z}";
    }

    public void SetObject(CelestialObject obj)
    {
        currentObject = obj;
        obj.currentTile = this;
        obj.gridPosition = gridPosition;
    }

    public void Clear()
    {
        currentObject = null;
    }
}
