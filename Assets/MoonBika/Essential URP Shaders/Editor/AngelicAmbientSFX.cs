using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MoonBika
{
    public class AngelicAmbientSFXEditor : EditorWindow
    {
        public AudioSource audioSource;
        public float volume = 0.5f;
        public float reverbAmount = 0.8f;
        public float pitch = 1.0f;
        public float lowPassFrequency = 5000f; // Smooth high end for warmth
        public float fadeDuration = 2.0f;      // Smooth fade in/out
        public AudioClip[] harmonicTones;      // Add multiple soft harmonic tones
        public float randomPitchVariation = 0.1f; // Subtle pitch shifting for ethereal effect
        public float playInterval = 5.0f;      // Interval between sounds

        private float nextPlayTime;

        [MenuItem("Tools/Angelic Ambient SFX")]
        public static void ShowWindow()
        {
            GetWindow<AngelicAmbientSFXEditor>("Angelic Ambient SFX");
        }

        private void OnGUI()
        {
            GUILayout.Label("Angelic Ambient SFX Settings", EditorStyles.boldLabel);

            audioSource = (AudioSource)EditorGUILayout.ObjectField("Audio Source", audioSource, typeof(AudioSource), true);
            volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
            reverbAmount = EditorGUILayout.Slider("Reverb Amount", reverbAmount, 0f, 1f);
            pitch = EditorGUILayout.Slider("Pitch", pitch, 0.5f, 2f);
            lowPassFrequency = EditorGUILayout.FloatField("Low Pass Frequency", lowPassFrequency);
            fadeDuration = EditorGUILayout.FloatField("Fade Duration", fadeDuration);
            playInterval = EditorGUILayout.FloatField("Play Interval", playInterval);
            randomPitchVariation = EditorGUILayout.Slider("Random Pitch Variation", randomPitchVariation, 0f, 0.5f);

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty harmonicTonesProp = serializedObject.FindProperty("harmonicTones");
            EditorGUILayout.PropertyField(harmonicTonesProp, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Play Angelic Sound"))
            {
                if (audioSource != null)
                {
                    PlayAngelicTone();
                }
                else
                {
                    Debug.LogError("AudioSource is not assigned!");
                }
            }

            if (GUILayout.Button("Stop Audio"))
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        void PlayAngelicTone()
        {
            if (harmonicTones.Length > 0)
            {
                AudioClip clip = harmonicTones[Random.Range(0, harmonicTones.Length)];
                audioSource.pitch = pitch + Random.Range(-randomPitchVariation, randomPitchVariation);
                audioSource.volume = volume;
                audioSource.PlayOneShot(clip);

                AddReverb();
                AddLowPassFilter();
            }
        }

        void AddReverb()
        {
            AudioReverbFilter reverb = audioSource.gameObject.GetComponent<AudioReverbFilter>() ?? audioSource.gameObject.AddComponent<AudioReverbFilter>();
            reverb.reverbLevel = reverbAmount * 1000;  // Increase for more ethereal effect
            reverb.decayTime = 3.0f;                   // Length of reverb
            reverb.roomHF = -100;                      // Soften high frequencies
        }

        void AddLowPassFilter()
        {
            AudioLowPassFilter lowPass = audioSource.gameObject.GetComponent<AudioLowPassFilter>() ?? audioSource.gameObject.AddComponent<AudioLowPassFilter>();
            lowPass.cutoffFrequency = lowPassFrequency;
        }
    }
}