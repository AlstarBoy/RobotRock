using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MoonBika
{
    public class AutoSaveEditor : EditorWindow
    {
        private bool autoSaveEnabled = false;
        private float saveInterval = 300f; // Save every 5 minutes
        private float nextSaveTime;
        private bool showNotification = true;
        private Object[] scenesToSave; // Array of scenes to save

        [MenuItem("Tools/Auto Save Settings")]
        public static void ShowWindow()
        {
            GetWindow<AutoSaveEditor>("Auto Save Settings");
        }

        private void OnEnable()
        {
            nextSaveTime = (float)EditorApplication.timeSinceStartup + saveInterval;
        }

        private void OnGUI()
        {
            GUILayout.Label("Auto Save Settings", EditorStyles.boldLabel);
            autoSaveEnabled = EditorGUILayout.Toggle("Enable Auto Save", autoSaveEnabled);
            saveInterval = EditorGUILayout.FloatField("Save Interval (seconds)", saveInterval);
            showNotification = EditorGUILayout.Toggle("Show Notification", showNotification);

            GUILayout.Label("Scenes to Auto-Save:", EditorStyles.boldLabel);
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("scenesToSave");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (autoSaveEnabled && EditorApplication.timeSinceStartup >= nextSaveTime)
            {
                SaveProject();
                nextSaveTime = (float)EditorApplication.timeSinceStartup + saveInterval;
            }
        }

        private void SaveProject()
        {
            Debug.Log("Auto-saving project...");

            // Save all specified scenes
            if (scenesToSave != null && scenesToSave.Length > 0)
            {
                foreach (Object scene in scenesToSave)
                {
                    if (scene != null)
                    {
                        string scenePath = AssetDatabase.GetAssetPath(scene);
                        if (scenePath.EndsWith(".unity"))
                        {
                            var sceneAsset = EditorSceneManager.GetSceneByPath(scenePath);
                            if (sceneAsset.IsValid())
                            {
                                EditorSceneManager.SaveScene(sceneAsset);
                                Debug.Log($"Scene saved: {scenePath}");
                            }
                        }
                    }
                }
            }
            else
            {
                // Save the active scene if no scenes are specified
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("Active scenes saved.");
            }

            AssetDatabase.SaveAssets();

            if (showNotification)
            {
                ShowNotification(new GUIContent("Project Auto-Saved!"));
            }
        }
    }
}