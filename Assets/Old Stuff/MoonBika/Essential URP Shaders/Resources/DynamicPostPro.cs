using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Use URP namespace

namespace MoonBika
{
    public class DynamicPostPro : MonoBehaviour
    {
        public Volume volume; // Reference to the Volume component
        public Light sun; // Reference to the Light (Sun) component

        // Define variables for URP effects
        private ColorAdjustments colorAdjustments;
        private Vignette vignette;
        private Bloom bloom;

        // Public variables to control the effects
        public bool enableColorAdjustments = true;
        public bool enableVignette = true;
        public bool enableBloom = true;

        // Sliders for effect adjustments
        [Range(-1f, 1f)]
        public float colorExposureMin = -1f;
        [Range(-1f, 1f)]
        public float colorExposureMax = 1f;

        [Range(-50f, 50f)]
        public float contrastMin = -50f;
        [Range(-50f, 50f)]
        public float contrastMax = 50f;

        [Range(0.1f, 1f)]
        public float vignetteMin = 0.1f;
        [Range(0.1f, 1f)]
        public float vignetteMax = 0.3f;

        [Range(0.2f, 2f)]
        public float bloomMin = 0.2f;
        [Range(0.2f, 2f)]
        public float bloomMax = 1.5f;

        void Start()
        {
            if (volume == null)
            {
                Debug.LogError("Volume is not assigned.");
                return;
            }

            // Get the volume profile and check for specific effects
            if (volume.profile == null)
            {
                Debug.LogError("Volume profile is not assigned.");
                return;
            }

            if (volume.profile.TryGet(out colorAdjustments))
            {
                Debug.Log("ColorAdjustments found");
            }
            else
            {
                Debug.LogWarning("ColorAdjustments not found in profile.");
            }

            if (volume.profile.TryGet(out vignette))
            {
                Debug.Log("Vignette found");
            }
            else
            {
                Debug.LogWarning("Vignette not found in profile.");
            }

            if (volume.profile.TryGet(out bloom))
            {
                Debug.Log("Bloom found");
            }
            else
            {
                Debug.LogWarning("Bloom not found in profile.");
            }
        }

        void Update()
        {
            if (sun == null)
            {
                Debug.LogError("Sun is not assigned.");
                return;
            }

            // Use the sun's rotation and color to adjust the post-processing effects
            AdjustPostProcessingEffects(sun.transform.eulerAngles, sun.color);
        }

        private void AdjustPostProcessingEffects(Vector3 sunRotation, Color sunColor)
        {
            // Example: Using the X rotation as a proxy for the angle
            float sunAngle = sunRotation.x;

            if (colorAdjustments != null && enableColorAdjustments)
            {
                // Adjust exposure based on the sun's angle
                colorAdjustments.postExposure.value = Mathf.Lerp(colorExposureMin, colorExposureMax, Mathf.InverseLerp(0f, 90f, sunAngle));

                // Adjust color filter for day/night cycle using the sun's color
                colorAdjustments.colorFilter.value = Color.Lerp(Color.blue, sunColor, Mathf.InverseLerp(0f, 90f, sunAngle));

                // Adjust contrast
                colorAdjustments.contrast.value = Mathf.Lerp(contrastMin, contrastMax, Mathf.InverseLerp(0f, 90f, sunAngle));
            }

            if (vignette != null && enableVignette)
            {
                // Increase vignette intensity during night time (assuming sunAngle near 0 is night)
                vignette.intensity.value = Mathf.Lerp(vignetteMin, vignetteMax, Mathf.InverseLerp(0f, 45f, sunAngle));
            }

            if (bloom != null && enableBloom)
            {
                // Enhance bloom during daytime and reduce it at night
                bloom.intensity.value = Mathf.Lerp(bloomMin, bloomMax, Mathf.InverseLerp(0f, 90f, sunAngle));
            }
        }
    }
}