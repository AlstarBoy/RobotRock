using UnityEngine;
using UnityEditor;

namespace MoonBika
{
    public class PathwayGenerator : EditorWindow
    {
        private int pointCount = 10;
        private float curveIntensity = 1.0f;
        private GameObject prefab;

        [MenuItem("Tools/Pathway Generator")]
        public static void ShowWindow()
        {
            GetWindow<PathwayGenerator>("Pathway Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Pathway Generator", EditorStyles.boldLabel);
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
            pointCount = EditorGUILayout.IntField("Point Count", pointCount);
            curveIntensity = EditorGUILayout.FloatField("Curve Intensity", curveIntensity);

            if (GUILayout.Button("Generate Path"))
            {
                GeneratePath();
            }
        }

        private void GeneratePath()
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is not assigned!");
                return;
            }

            // Create a parent GameObject to organize the pathway
            GameObject pathwayParent = new GameObject("PathwayParent");

            Vector3 previousPoint = Vector3.zero;

            for (int i = 0; i < pointCount; i++)
            {
                float offsetX = Mathf.Sin(i * curveIntensity) * 5f;
                Vector3 point = new Vector3(previousPoint.x + offsetX, 0, previousPoint.z + 5f);

                // Instantiate the prefab as a child of the pathway parent
                GameObject instantiatedObject = Instantiate(prefab, point, Quaternion.identity, pathwayParent.transform);

                previousPoint = point;
            }

            // Optionally select the parent object in the hierarchy after generation
            Selection.activeGameObject = pathwayParent;
        }
    }
}