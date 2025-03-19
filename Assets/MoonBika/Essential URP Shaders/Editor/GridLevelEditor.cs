using UnityEngine;
using UnityEditor;

namespace MoonBika
{
    public class GridLevelEditor : EditorWindow
    {
        private GameObject tilePrefab;
        private int gridWidth = 10;
        private int gridHeight = 10;
        private float tileSize = 1.0f;

        // New options for randomization
        private bool randomizePosition = false;
        private bool randomizeRotation = false;
        private Vector3 randomPositionRange = new Vector3(0.5f, 0, 0.5f); // Randomization range for position
        private Vector3 randomRotationRange = new Vector3(0, 360, 0); // Randomization range for rotation

        [MenuItem("Tools/Grid Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<GridLevelEditor>("Grid Level Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Grid Level Editor", EditorStyles.boldLabel);

            tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
            gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
            gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);
            tileSize = EditorGUILayout.FloatField("Tile Size", tileSize);

            // Options for randomizing position and rotation
            randomizePosition = EditorGUILayout.Toggle("Randomize Position", randomizePosition);
            if (randomizePosition)
            {
                randomPositionRange = EditorGUILayout.Vector3Field("Position Randomization Range", randomPositionRange);
            }

            randomizeRotation = EditorGUILayout.Toggle("Randomize Rotation", randomizeRotation);
            if (randomizeRotation)
            {
                randomRotationRange = EditorGUILayout.Vector3Field("Rotation Randomization Range", randomRotationRange);
            }

            if (GUILayout.Button("Generate Grid"))
            {
                GenerateGrid();
            }
        }

        private void GenerateGrid()
        {
            if (tilePrefab == null)
            {
                Debug.LogError("Tile Prefab is not assigned!");
                return;
            }

            // Create a parent GameObject to organize the tiles
            GameObject parentObject = new GameObject("GridParent");

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                    // Randomize position
                    if (randomizePosition)
                    {
                        position.x += Random.Range(-randomPositionRange.x, randomPositionRange.x);
                        position.z += Random.Range(-randomPositionRange.z, randomPositionRange.z);
                    }

                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    tile.name = "Tile_" + x + "_" + z;

                    // Randomize rotation
                    if (randomizeRotation)
                    {
                        float randomYRotation = Random.Range(-randomRotationRange.y, randomRotationRange.y);
                        tile.transform.rotation = Quaternion.Euler(0, randomYRotation, 0);
                    }

                    // Set the parent of the tile to the parentObject
                    tile.transform.parent = parentObject.transform;
                }
            }
        }
    }
}