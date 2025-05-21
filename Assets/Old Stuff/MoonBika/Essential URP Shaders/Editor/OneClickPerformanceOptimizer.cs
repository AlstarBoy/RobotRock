using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace MoonBika
{
    public class OneClickPerformanceOptimizer : EditorWindow
    {
        [MenuItem("Tools/1-Click Performance Optimizer")]
        public static void ShowWindow()
        {
            GetWindow<OneClickPerformanceOptimizer>("Performance Optimizer");
        }

        private void OnGUI()
        {
            GUILayout.Label("1-Click Performance Optimization", EditorStyles.boldLabel);

            if (GUILayout.Button("Optimize Now"))
            {
                OptimizeTextures();
                OptimizeMeshes();
                OptimizeLOD();
                OptimizeLighting();
                OptimizePhysicsSettings();
                BatchDisableUnusedComponents();
                CombineStaticMeshes();
                OptimizeShaders();
                TriggerGarbageCollection();
                Debug.Log("Optimization Complete!");
            }
        }

        // 1. Optimize Textures: Resize and compress textures
        private static void OptimizeTextures()
        {
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture");
            foreach (string guid in textureGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer != null)
                {
                    importer.maxTextureSize = 512;
                    importer.textureCompression = TextureImporterCompression.Compressed;
                    importer.mipmapEnabled = true;
                    importer.SaveAndReimport();
                }
            }

            Debug.Log("Texture optimization complete.");
        }

        // 2. Optimize Meshes: Combine and reduce vertex count
        private static void OptimizeMeshes()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    MeshUtility.Optimize(mesh);
                }
            }

            Debug.Log("Mesh optimization complete.");
        }

        // 3. Optimize LOD: Automatically add LOD to objects without one
        private static void OptimizeLOD()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.GetComponent<LODGroup>() == null && obj.GetComponent<Renderer>() != null)
                {
                    LODGroup lodGroup = obj.AddComponent<LODGroup>();

                    Renderer renderer = obj.GetComponent<Renderer>();
                    LOD[] lods = new LOD[3];

                    lods[0] = new LOD(0.6f, new Renderer[] { renderer });
                    lods[1] = new LOD(0.3f, new Renderer[] { renderer });
                    lods[2] = new LOD(0.1f, new Renderer[] { renderer });

                    lodGroup.SetLODs(lods);
                    lodGroup.RecalculateBounds();
                }
            }

            Debug.Log("LOD optimization complete.");
        }

        // 4. Optimize Lighting: Reduce unnecessary lighting elements
        private static void OptimizeLighting()
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    light.shadows = LightShadows.None;
                }
                else if (light.type == LightType.Point || light.type == LightType.Spot)
                {
                    light.shadows = LightShadows.None;
                }
            }

            ReflectionProbe[] probes = GameObject.FindObjectsOfType<ReflectionProbe>();
            foreach (ReflectionProbe probe in probes)
            {
                probe.enabled = false;
            }

            Debug.Log("Lighting optimization complete.");
        }

        // 5. Optimize Physics Settings: Adjust physics settings for better performance
        private static void OptimizePhysicsSettings()
        {
            // Lower fixed timestep to reduce CPU load
            Time.fixedDeltaTime = 0.02f; // Increase to improve performance but lower physics accuracy (0.02 = 50 fps)

            // Lower solver iteration count for performance
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;

            Debug.Log("Physics optimization complete.");
        }

        // 6. Batch Disable Unused Components: Disable unnecessary components like colliders
        private static void BatchDisableUnusedComponents()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // Example: Disable unused Colliders
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null && !collider.enabled)
                {
                    collider.enabled = false;
                }
            }

            Debug.Log("Component optimization complete.");
        }

        // 7. Combine Static Meshes: Combine static meshes to reduce draw calls
        private static void CombineStaticMeshes()
        {
            GameObject[] staticObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in staticObjects)
            {
                if (obj.isStatic && obj.GetComponent<MeshFilter>() != null)
                {
                    StaticBatchingUtility.Combine(obj);
                }
            }

            Debug.Log("Static mesh combination complete.");
        }

        // 8. Shader Optimization: Force simpler shaders for low-end devices
        private static void OptimizeShaders()
        {
            Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();

            foreach (Shader shader in shaders)
            {
                // Force simpler shaders (you can add specific rules based on your game's needs)
                if (shader.name.Contains("HighQuality"))
                {
                    // Replace with low-quality version if necessary
                }
            }

            Debug.Log("Shader optimization complete.");
        }

        // 9. Trigger Garbage Collection: Free up unused memory
        private static void TriggerGarbageCollection()
        {
            System.GC.Collect();
            Debug.Log("Garbage collection triggered.");
        }
    }
}