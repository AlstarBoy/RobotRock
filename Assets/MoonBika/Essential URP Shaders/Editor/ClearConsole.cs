using UnityEditor;
using UnityEngine;

namespace MoonBika
{
    public static class ClearConsole
    {
        [MenuItem("Tools/Clear Console %#c")] // Ctrl+Shift+C
        public static void Clear()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }
}