using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MoonBika
{
    public class MiniToDoList : EditorWindow
    {
        private List<string> tasks = new List<string>();
        private List<bool> taskStatus = new List<bool>();
        private string newTask = "";

        [MenuItem("Window/Mini To-Do List")]
        public static void ShowWindow()
        {
            GetWindow<MiniToDoList>("To-Do List");
        }

        private void OnGUI()
        {
            GUILayout.Label("Mini To-Do List", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            newTask = EditorGUILayout.TextField(newTask);
            if (GUILayout.Button("Add Task"))
            {
                if (!string.IsNullOrEmpty(newTask))
                {
                    tasks.Add(newTask);
                    taskStatus.Add(false);
                    newTask = "";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            for (int i = 0; i < tasks.Count; i++)
            {
                GUILayout.BeginHorizontal();
                taskStatus[i] = EditorGUILayout.Toggle(taskStatus[i], GUILayout.Width(20));
                GUIStyle taskStyle = taskStatus[i] ? new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic, normal = { textColor = Color.gray } } : EditorStyles.label;
                GUILayout.Label(tasks[i], taskStyle);

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    tasks.RemoveAt(i);
                    taskStatus.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            if (tasks.Count == 0)
            {
                GUILayout.Label("No tasks yet. Add a new task above.", EditorStyles.miniLabel);
            }
        }
    }}