using UnityEditor;
using UnityEngine;

namespace MoonBika
{
    public class SimpleTimerEditor : EditorWindow
    {
        private static float startTime;
        private static float elapsedTime;
        private static bool isRunning;

        [MenuItem("Window/Simple Timer")]
        public static void ShowWindow()
        {
            GetWindow<SimpleTimerEditor>("Simple Timer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Timer", EditorStyles.boldLabel);

            float minutes = Mathf.FloorToInt(elapsedTime / 60f);
            float seconds = elapsedTime % 60f;
            EditorGUILayout.LabelField("Elapsed Time:", string.Format("{0:00}:{1:00}", minutes, seconds));

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(isRunning ? "Stop" : "Start"))
            {
                if (isRunning)
                    StopTimer();
                else
                    StartTimer();
            }

            if (GUILayout.Button("Reset"))
            {
                ResetTimer();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            EditorApplication.update += UpdateTimer;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateTimer;
        }

        private void UpdateTimer()
        {
            if (isRunning)
            {
                elapsedTime = Time.realtimeSinceStartup - startTime;
                Repaint(); // Refresh the GUI
            }
        }

        private void StartTimer()
        {
            startTime = Time.realtimeSinceStartup - elapsedTime;
            isRunning = true;
        }

        private void StopTimer()
        {
            isRunning = false;
        }

        private void ResetTimer()
        {
            startTime = Time.realtimeSinceStartup;
            elapsedTime = 0f;
            isRunning = false;
        }
    }
}