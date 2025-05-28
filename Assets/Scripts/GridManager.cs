
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the grid and provides neighbor lookup for XZ-based tile maps.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    private void Awake()
    {
        Instance = this;
    }

    public Tile GetTile(Vector2Int pos)
    {
        return tiles.ContainsKey(pos) ? tiles[pos] : null;
    }

    public Tile GetClosestTile(Vector2 pos)
    {
        float minDist = float.MaxValue;
        Tile closest = null;
        foreach (var tile in tiles.Values)
        {
            Vector2 tileXZ = new Vector2(tile.transform.position.x, tile.transform.position.z);
            float dist = Vector2.Distance(tileXZ, pos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = tile;
            }
        }
        return closest;
    }

    public List<Tile> GetAllNeighbors(Tile tile)
    {
        var dirs = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(-1, -1),
            new Vector2Int(-1, 1), new Vector2Int(1, -1)
        };

        List<Tile> neighbors = new List<Tile>();
        foreach (var dir in dirs)
        {
            Vector2Int pos = tile.gridPosition + dir;
            if (tiles.ContainsKey(pos)) neighbors.Add(tiles[pos]);
        }

        return neighbors;
    }
}
