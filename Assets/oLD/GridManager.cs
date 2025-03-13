using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private GameObject _tilePrefab; // Prefab should have a SpriteRenderer
    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, GameObject> _tiles;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, GameObject>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Spawn tiles in the XY plane (Z=0)
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                // Change tile color in a checkerboard pattern
                var renderer = spawnedTile.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                    renderer.color = isOffset ? Color.gray : Color.white;
                }

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
    }

    public GameObject GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}
