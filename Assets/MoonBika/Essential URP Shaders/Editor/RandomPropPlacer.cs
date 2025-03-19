using UnityEngine;
using UnityEditor;

namespace MoonBika
{
    public class RandomPropPlacer : EditorWindow
    {
        public enum ShapeType { Cube, Sphere, Cylinder, Capsule, Custom }

        public ShapeType shape = ShapeType.Cube;
        public GameObject customModel; // Field for custom model
        public int objectCount = 10;
        public Vector2 rangeX = new Vector2(-10, 10);
        public Vector2 rangeZ = new Vector2(-10, 10);

        // New parameters
        public bool randomSize = false;
        public Vector2 sizeRange = new Vector2(1, 3);
        public bool randomRotation = false;
        public bool adjustLight = false;
        public float minDistance = 1.0f; // Minimum distance between objects to avoid overlap

        [MenuItem("Tools/Random Prop Placer")]
        public static void ShowWindow()
        {
            GetWindow<RandomPropPlacer>("Random Prop Placer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Random Prop Placer", EditorStyles.boldLabel);

            shape = (ShapeType)EditorGUILayout.EnumPopup("Shape", shape);

            if (shape == ShapeType.Custom)
            {
                customModel = (GameObject)EditorGUILayout.ObjectField("Custom Model", customModel, typeof(GameObject), false);
            }

            objectCount = EditorGUILayout.IntField("Object Count", objectCount);
            rangeX = EditorGUILayout.Vector2Field("X Range", rangeX);
            rangeZ = EditorGUILayout.Vector2Field("Z Range", rangeZ);

            randomSize = EditorGUILayout.Toggle("Random Size", randomSize);
            if (randomSize)
            {
                sizeRange = EditorGUILayout.Vector2Field("Size Range", sizeRange);
            }

            randomRotation = EditorGUILayout.Toggle("Random Rotation", randomRotation);

            adjustLight = EditorGUILayout.Toggle("Adjust Light", adjustLight);

            minDistance = EditorGUILayout.FloatField("Min Distance Between Objects", minDistance);

            if (GUILayout.Button("Place Objects"))
            {
                PlaceObjects();
            }
        }

        private void PlaceObjects()
        {
            // Optionally adjust light
            if (adjustLight)
            {
                Light light = GameObject.FindObjectOfType<Light>();
                if (light != null)
                {
                    light.intensity = 1.5f; // Adjust as needed
                    light.color = Color.white;
                }
                else
                {
                    Debug.LogWarning("No light found in the scene. Adjust lighting manually if needed.");
                }
            }

            // Create a parent GameObject to organize the placed objects
            GameObject parentObject = new GameObject("RandomProps");

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 position;
                bool validPosition;

                do
                {
                    position = new Vector3(
                        Random.Range(rangeX.x, rangeX.y),
                        0,
                        Random.Range(rangeZ.x, rangeZ.y)
                    );

                    validPosition = IsValidPosition(position);
                }
                while (!validPosition);

                GameObject newObject = CreateShape(shape);
                newObject.transform.position = position;
                newObject.transform.SetParent(parentObject.transform); // Set parent

                if (randomSize)
                {
                    float randomScale = Random.Range(sizeRange.x, sizeRange.y);
                    newObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                }

                if (randomRotation)
                {
                    newObject.transform.rotation = Quaternion.Euler(
                        Random.Range(0, 360),
                        Random.Range(0, 360),
                        Random.Range(0, 360)
                    );
                }
            }
        }

        private GameObject CreateShape(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Cube:
                    return GameObject.CreatePrimitive(PrimitiveType.Cube);
                case ShapeType.Sphere:
                    return GameObject.CreatePrimitive(PrimitiveType.Sphere);
                case ShapeType.Cylinder:
                    return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                case ShapeType.Capsule:
                    return GameObject.CreatePrimitive(PrimitiveType.Capsule);
                case ShapeType.Custom:
                    if (customModel != null)
                    {
                        return Instantiate(customModel);
                    }
                    else
                    {
                        Debug.LogError("Custom model is not assigned!");
                        return null;
                    }
                default:
                    return null;
            }
        }

        private bool IsValidPosition(Vector3 position)
        {
            // Check if the position is too close to any existing objects
            Collider[] colliders = Physics.OverlapSphere(position, minDistance);
            return colliders.Length == 0; // Valid if no colliders are detected within the min distance
        }
    }
}