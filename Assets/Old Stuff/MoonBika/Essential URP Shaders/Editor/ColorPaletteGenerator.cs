using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace MoonBika
{
    public class ColorPaletteGenerator : EditorWindow
    {
        private int paletteSize = 5;
        private Color[] colors;
        private Material[] materials;

        private enum GenerationType
        {
            Random,
            Gradient,
            ReverseGradient
        }

        private GenerationType generationType = GenerationType.Random;

        [MenuItem("Tools/Color Palette Generator")]
        public static void ShowWindow()
        {
            GetWindow<ColorPaletteGenerator>("Color Palette Generator");
        }

        private void OnEnable()
        {
            colors = new Color[paletteSize];
            materials = new Material[paletteSize];
            GenerateColors();
        }

        private void OnGUI()
        {
            GUILayout.Label("Color Palette Generator", EditorStyles.boldLabel);

            paletteSize = EditorGUILayout.IntField("Palette Size", paletteSize);
            generationType = (GenerationType)EditorGUILayout.EnumPopup("Generation Type", generationType);

            if (GUILayout.Button("Regenerate Palette"))
            {
                GenerateColors();
            }

            if (colors.Length > 0)
            {
                EditorGUILayout.LabelField("Palette Preview", EditorStyles.boldLabel);
                DrawPalette(colors);

                if (GUILayout.Button("Apply to Materials"))
                {
                    ApplyColorsToMaterials();
                }
            }
        }

        private void GenerateColors()
        {
            colors = new Color[paletteSize];

            switch (generationType)
            {
                case GenerationType.Random:
                    GenerateRandomColors();
                    break;
                case GenerationType.Gradient:
                    GenerateGradientColors();
                    break;
                case GenerationType.ReverseGradient:
                    GenerateReverseGradientColors();
                    break;
            }

            CreateMaterials();
        }

        private void GenerateRandomColors()
        {
            for (int i = 0; i < paletteSize; i++)
            {
                colors[i] = UnityEngine.Random.ColorHSV();
            }
        }

        private void GenerateGradientColors()
        {
            for (int i = 0; i < paletteSize; i++)
            {
                float t = i / (float)(paletteSize - 1);
                colors[i] = Color.Lerp(Color.red, Color.blue, t); // Example gradient from red to blue
            }
        }

        private void GenerateReverseGradientColors()
        {
            for (int i = 0; i < paletteSize; i++)
            {
                float t = i / (float)(paletteSize - 1);
                colors[i] = Color.Lerp(Color.blue, Color.red, t); // Example reversed gradient from blue to red
            }
        }

        private void CreateMaterials()
        {
            materials = new Material[paletteSize];
            string basePath = "Assets/GeneratedMaterials/";
            Directory.CreateDirectory(basePath);

            for (int i = 0; i < paletteSize; i++)
            {
                string path = $"{basePath}Material_{i}.mat";
                Material material = new Material(Shader.Find("Standard"));
                material.color = colors[i];
                AssetDatabase.CreateAsset(material, path);
                materials[i] = material;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ApplyColorsToMaterials()
        {
            foreach (var material in materials)
            {
                if (material != null)
                {
                    material.color = colors[Array.IndexOf(materials, material)];
                    EditorUtility.SetDirty(material);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void DrawPalette(Color[] colors)
        {
            int size = 100;
            int padding = 10;
            int totalWidth = (size + padding) * colors.Length;

            Texture2D texture = new Texture2D(totalWidth, size);
            Color[] textureColors = new Color[totalWidth * size];

            for (int i = 0; i < colors.Length; i++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        textureColors[(y * totalWidth) + (i * (size + padding)) + x] = colors[i];
                    }
                }
            }

            texture.SetPixels(textureColors);
            texture.Apply();

            GUILayout.Label(new GUIContent(texture), GUILayout.Width(totalWidth), GUILayout.Height(size));
        }
    }
}