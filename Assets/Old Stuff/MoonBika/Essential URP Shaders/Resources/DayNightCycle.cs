using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoonBika
{
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Sun and Moon Settings")]
        public Light sun;
        public Light moon;
        public float dayDurationInMinutes = 1f; // Duration of a full day in minutes

        [Header("Sun Settings")]
        public Gradient sunColor;
        public AnimationCurve sunIntensityCurve;
        public float sunMaxIntensity = 1.2f;

        [Header("Moon Settings")]
        public Gradient moonColor;
        public AnimationCurve moonIntensityCurve;
        public float moonMaxIntensity = 0.5f;

        [Header("Rotation Settings")]
        public float rotationOffset = -90f; // Offset to start the cycle correctly

        private float dayDurationInSeconds;
        private float timeOfDay;

        void Start()
        {
            dayDurationInSeconds = dayDurationInMinutes * 60f;
        }

        void Update()
        {
            // Calculate time progression
            timeOfDay += Time.deltaTime / dayDurationInSeconds;
            timeOfDay %= 1; // Keeps the timeOfDay value between 0 and 1

            // Rotate the sun and moon based on time of day
            float sunAngle = timeOfDay * 360f + rotationOffset;
            sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
            moon.transform.rotation = Quaternion.Euler(sunAngle + 180f, 170f, 0f);

            // Adjust sun and moon colors and intensities dynamically
            sun.intensity = sunIntensityCurve.Evaluate(timeOfDay) * sunMaxIntensity;
            sun.color = sunColor.Evaluate(timeOfDay);

            moon.intensity = moonIntensityCurve.Evaluate(timeOfDay) * moonMaxIntensity;
            moon.color = moonColor.Evaluate(timeOfDay);
        }
    }
}