using UnityEditor;
using UnityEngine;

namespace MoonBika
{
    public class OptimizeSceneHierarchy : EditorWindow
    {
        [MenuItem("Tools/Optimize Scene Hierarchy")]
        public static void ShowWindow()
        {
            GetWindow<OptimizeSceneHierarchy>("Optimize Scene Hierarchy");
        }

        private void OnGUI()
        {
            GUILayout.Label("Optimize Scene Hierarchy", EditorStyles.boldLabel);

            if (GUILayout.Button("Organize Hierarchy"))
            {
                OrganizeHierarchy();
            }
        }

        private void OrganizeHierarchy()
        {
            Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>();

            foreach (Transform transform in allTransforms)
            {
                if (transform.parent != null)
                {
                    transform.SetSiblingIndex(Random.Range(0, transform.parent.childCount));
                }
            }

            Debug.Log("Hierarchy optimization complete.");
        }
    }
}