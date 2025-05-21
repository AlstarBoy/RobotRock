using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MoonBika
{
    public class MultiAssetCreator : EditorWindow
    {
        private List<string> folderNames = new List<string>();
        private List<string> scriptNames = new List<string>();
        private List<string> scriptContents = new List<string>();
        private string defaultPath = "Assets";

        [MenuItem("Tools/Multi Asset Creator")]
        public static void ShowWindow()
        {
            GetWindow<MultiAssetCreator>("Multi Asset Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create Multiple Folders and Scripts", EditorStyles.boldLabel);

            // Default Path
            GUILayout.BeginHorizontal();
            GUILayout.Label("Default Path: ", GUILayout.Width(80));
            defaultPath = GUILayout.TextField(defaultPath);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Folders Section
            GUILayout.Label("Folders to Create:", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Folder"))
            {
                folderNames.Add("");
            }

            for (int i = 0; i < folderNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                folderNames[i] = GUILayout.TextField(folderNames[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    folderNames.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            // Scripts Section
            GUILayout.Label("Scripts to Create:", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Script"))
            {
                scriptNames.Add("");
                scriptContents.Add("using UnityEngine;\n\npublic class NewScript : MonoBehaviour\n{\n    void Start()\n    {\n        // Start logic here\n    }\n\n    void Update()\n    {\n        // Update logic here\n    }\n}");
            }

            for (int i = 0; i < scriptNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                scriptNames[i] = GUILayout.TextField(scriptNames[i], GUILayout.Width(150));
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    scriptNames.RemoveAt(i);
                    scriptContents.RemoveAt(i);
                }
                GUILayout.EndHorizontal();

                GUILayout.Label("Script Content:");
                scriptContents[i] = EditorGUILayout.TextArea(scriptContents[i], GUILayout.Height(100));
            }

            GUILayout.Space(20);

            // Create Assets Button
            if (GUILayout.Button("Create Assets"))
            {
                CreateFolders();
                CreateScripts();
                AssetDatabase.Refresh();
            }
        }

        private void CreateFolders()
        {
            foreach (string folderName in folderNames)
            {
                if (!string.IsNullOrEmpty(folderName))
                {
                    string fullPath = Path.Combine(defaultPath, folderName);
                    if (!AssetDatabase.IsValidFolder(fullPath))
                    {
                        AssetDatabase.CreateFolder(defaultPath, folderName);
                        Debug.Log($"Folder '{folderName}' created at '{defaultPath}'.");
                    }
                    else
                    {
                        Debug.LogWarning($"Folder '{folderName}' already exists at '{defaultPath}'.");
                    }
                }
            }
        }

        private void CreateScripts()
        {
            for (int i = 0; i < scriptNames.Count; i++)
            {
                string scriptName = scriptNames[i];
                string scriptContent = scriptContents[i];

                if (!string.IsNullOrEmpty(scriptName))
                {
                    string scriptPath = Path.Combine(defaultPath, scriptName + ".cs");
                    if (!File.Exists(scriptPath))
                    {
                        File.WriteAllText(scriptPath, scriptContent);
                        Debug.Log($"Script '{scriptName}' created at '{defaultPath}'.");
                    }
                    else
                    {
                        Debug.LogWarning($"Script '{scriptName}' already exists at '{defaultPath}'.");
                    }
                }
            }
        }
    }
}