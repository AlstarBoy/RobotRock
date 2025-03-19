using UnityEngine;
using UnityEditor;

namespace MoonBika
{
    [InitializeOnLoad]
    public class FolderCustomizerEditor : EditorWindow
    {
        private static Color color1;
        private static Color color2;

        private const string Color1Key = "FolderCustomizer_Color1";
        private const string Color2Key = "FolderCustomizer_Color2";

        static FolderCustomizerEditor()
        {
            // Load saved colors or use defaults
            color1 = LoadColor(Color1Key, new Color(0.1f, 0.1f, 0.1f, 0.2f));
            color2 = LoadColor(Color2Key, new Color(0.5f, 0.5f, 0.5f, 0.2f));

            // Register to the project window update callback
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        [MenuItem("Window/Folder Customizer")]
        public static void ShowWindow()
        {
            GetWindow<FolderCustomizerEditor>("Folder Customizer");
        }

        private void OnEnable()
        {
            // Load saved colors when window is opened
            color1 = LoadColor(Color1Key, color1);
            color2 = LoadColor(Color2Key, color2);
        }

        private void OnDisable()
        {
            // Save the colors when window is closed
            SaveColor(Color1Key, color1);
            SaveColor(Color2Key, color2);
        }

        private void OnGUI()
        {
            GUILayout.Label("Folder Customizer Settings", EditorStyles.boldLabel);

            // Color pickers for alternating folder colors
            color1 = EditorGUILayout.ColorField("Color 1", color1);
            color2 = EditorGUILayout.ColorField("Color 2", color2);

            // Apply button to manually refresh the project window
            if (GUILayout.Button("Apply"))
            {
                ApplyChanges();
            }
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            // Get the path of the selected item
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                // Alternate folder background colors for better visibility
                Color folderColor = selectionRect.y % 2 == 0 ? color1 : color2;

                // Draw the background color
                EditorGUI.DrawRect(selectionRect, folderColor);

                // Draw the default folder icon and label without additional customizations
                var icon = EditorGUIUtility.IconContent("Folder Icon").image;
                GUI.DrawTexture(new Rect(selectionRect.x + 2, selectionRect.y, 16, 16), icon);
            }
        }

        private static void ApplyChanges()
        {
            // Force a refresh of the project window to apply changes
            EditorApplication.RepaintProjectWindow();
        }

        private static void SaveColor(string key, Color color)
        {
            EditorPrefs.SetFloat(key + "_r", color.r);
            EditorPrefs.SetFloat(key + "_g", color.g);
            EditorPrefs.SetFloat(key + "_b", color.b);
            EditorPrefs.SetFloat(key + "_a", color.a);
        }

        private static Color LoadColor(string key, Color defaultColor)
        {
            float r = EditorPrefs.GetFloat(key + "_r", defaultColor.r);
            float g = EditorPrefs.GetFloat(key + "_g", defaultColor.g);
            float b = EditorPrefs.GetFloat(key + "_b", defaultColor.b);
            float a = EditorPrefs.GetFloat(key + "_a", defaultColor.a);
            return new Color(r, g, b, a);
        }
    }
}