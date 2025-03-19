using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MoonBika
{
    public class SimpleToDoList : EditorWindow
    {
        private const string TaskListKey = "TaskList";

        private List<string> tasks = new List<string>();
        private string newTask = "";
        private Vector2 scrollPos;

        [MenuItem("Window/Simple To-Do List")]
        public static void ShowWindow()
        {
            GetWindow<SimpleToDoList>("Simple To-Do List");
        }

        private void OnEnable()
        {
            LoadTaskList();
        }

        private void OnGUI()
        {
            GUILayout.Label("To-Do List", EditorStyles.boldLabel);

            // Scroll view for the task list
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < tasks.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Display task text
                EditorGUILayout.LabelField(tasks[i]);

                // Button to remove task
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    RemoveTask(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // Input field for adding new tasks
            newTask = EditorGUILayout.TextField("New Task", newTask);

            // Detect Enter key press to add the task quickly
            Event e = Event.current;
            if (e.isKey && e.keyCode == KeyCode.Return && !string.IsNullOrEmpty(newTask))
            {
                AddTask(newTask);
                newTask = "";
                e.Use(); // Mark the event as used
            }

            // Button to add new task
            if (GUILayout.Button("Add Task") && !string.IsNullOrEmpty(newTask))
            {
                AddTask(newTask);
                newTask = "";
            }
        }

        private void LoadTaskList()
        {
            string savedData = PlayerPrefs.GetString(TaskListKey);
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] taskList = savedData.Split(',');
                tasks = new List<string>(taskList);
            }
        }

        private void SaveTaskList()
        {
            string dataToSave = string.Join(",", tasks);
            PlayerPrefs.SetString(TaskListKey, dataToSave);
            PlayerPrefs.Save();
        }

        private void AddTask(string task)
        {
            tasks.Add(task);
            SaveTaskList();
        }

        private void RemoveTask(int index)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks.RemoveAt(index);
                SaveTaskList();
            }
        }
    }
}