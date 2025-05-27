
using UnityEngine;

/// <summary>
/// Represents a tile in the grid.
/// </summary>
public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public CelestialObject currentObject;

    public bool IsOccupied => currentObject != null;

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
