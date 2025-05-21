using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoonBika
{
    public class SkyboxFlipper : MonoBehaviour
    {
        [Header("Skybox Settings")]
        public Material[] skyboxes; // Array of skyboxes to switch between
        public float switchInterval = 30f; // Time in seconds between skybox changes

        private int currentSkyboxIndex = 0;
        private float timer = 0f;

        void Start()
        {
            if (skyboxes.Length > 0)
            {
                // Set the initial skybox
                RenderSettings.skybox = skyboxes[currentSkyboxIndex];
            }
        }

        void Update()
        {
            if (skyboxes.Length == 0)
                return; // Exit if no skyboxes are assigned

            // Increment the timer
            timer += Time.deltaTime;

            // Check if it's time to switch skyboxes
            if (timer >= switchInterval)
            {
                timer = 0f;
                SwitchSkybox();
            }
        }

        void SwitchSkybox()
        {
            // Cycle to the next skybox in the array
            currentSkyboxIndex = (currentSkyboxIndex + 1) % skyboxes.Length;
            RenderSettings.skybox = skyboxes[currentSkyboxIndex];
            DynamicGI.UpdateEnvironment(); // Update the lighting to match the new skybox
        }
    }
}