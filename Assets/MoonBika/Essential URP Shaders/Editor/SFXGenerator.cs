using UnityEditor;
using UnityEngine;
using System.IO;

namespace MoonBika
{
    public class SFXGenerator : EditorWindow
    {
        private float frequency = 440f; // A4 note
        private float duration = 1f; // 1 second
        private int sampleRate = 44100; // CD quality
        private float amplitude = 0.5f; // Volume
        private AnimationCurve attackCurve = AnimationCurve.Linear(0, 0, 1, 1); // Attack curve
        private AnimationCurve decayCurve = AnimationCurve.Linear(0, 1, 1, 0); // Decay curve
        private float sustainLevel = 0.7f; // Sustain level
        private float release = 0.1f; // Release time
        private float pitch = 1f; // Pitch multiplier
        private string waveform = "Sine";
        private string savePath = "Assets/GeneratedAudio/";

        private static readonly string[] waveforms = { "Sine", "Square", "Sawtooth", "Noise" };

        [MenuItem("Tools/SFX Generator")]
        public static void ShowWindow()
        {
            GetWindow<SFXGenerator>("SFX Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("SFX Generator", EditorStyles.boldLabel);

            frequency = EditorGUILayout.FloatField("Frequency (Hz)", frequency);
            duration = EditorGUILayout.FloatField("Duration (s)", duration);
            amplitude = EditorGUILayout.Slider("Amplitude", amplitude, 0f, 1f);
            attackCurve = EditorGUILayout.CurveField("Attack Curve", attackCurve);
            decayCurve = EditorGUILayout.CurveField("Decay Curve", decayCurve);
            sustainLevel = EditorGUILayout.Slider("Sustain Level", sustainLevel, 0f, 1f);
            release = EditorGUILayout.FloatField("Release (s)", release);
            pitch = EditorGUILayout.FloatField("Pitch", pitch);
            waveform = waveforms[EditorGUILayout.Popup("Waveform", GetWaveformIndex(waveform), waveforms)];
            savePath = EditorGUILayout.TextField("Save Path", savePath);

            if (GUILayout.Button("Generate SFX"))
            {
                GenerateSFX();
            }
        }

        private void GenerateSFX()
        {
            int sampleCount = Mathf.CeilToInt(duration * sampleRate);
            AudioClip audioClip = AudioClip.Create("GeneratedSFX", sampleCount, 1, sampleRate, false);
            float[] samples = new float[sampleCount];

            float attackSamples = attackCurve.keys[attackCurve.length - 1].time * sampleRate;
            float decaySamples = duration * sampleRate - attackSamples - release * sampleRate;

            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / sampleRate;
                float sampleValue = 0f;

                switch (waveform)
                {
                    case "Sine":
                        sampleValue = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * time * pitch);
                        break;
                    case "Square":
                        sampleValue = amplitude * Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * time * pitch));
                        break;
                    case "Sawtooth":
                        sampleValue = amplitude * (2f * (time * frequency * pitch - Mathf.Floor(time * frequency * pitch + 0.5f)));
                        break;
                    case "Noise":
                        sampleValue = amplitude * Random.Range(-1f, 1f);
                        break;
                }

                // Apply ADSR envelope with curves
                float envelope = 1f;
                if (i < attackSamples)
                {
                    envelope = attackCurve.Evaluate((float)i / attackSamples);
                }
                else if (i < attackSamples + decaySamples)
                {
                    envelope = decayCurve.Evaluate((float)(i - attackSamples) / decaySamples) * sustainLevel / amplitude;
                }
                else if (i < sampleCount - release * sampleRate)
                {
                    envelope = sustainLevel / amplitude;
                }
                else
                {
                    envelope = sustainLevel / amplitude * (1f - (float)(i - (sampleCount - release * sampleRate)) / (release * sampleRate));
                }

                samples[i] = sampleValue * envelope;
            }

            audioClip.SetData(samples, 0);
            SaveClip(audioClip, "GeneratedSFX");
            Debug.Log("SFX Generated and saved!");
        }

        private int GetWaveformIndex(string waveform)
        {
            for (int i = 0; i < waveforms.Length; i++)
            {
                if (waveforms[i] == waveform)
                {
                    return i;
                }
            }
            return 0; // Default to "Sine"
        }

        private void SaveClip(AudioClip clip, string fileName)
        {
            string path = Path.Combine(savePath, fileName + ".wav");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                int sampleCount = clip.samples * clip.channels;
                float[] samples = new float[sampleCount];
                clip.GetData(samples, 0);

                WriteWavHeader(binaryWriter, clip.frequency, clip.channels, sampleCount);
                WriteWavData(binaryWriter, samples);
            }

            AssetDatabase.ImportAsset(path);
            Debug.Log("AudioClip saved to " + path);
        }

        private static void WriteWavHeader(BinaryWriter writer, int sampleRate, int channels, int sampleCount)
        {
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + sampleCount * 2);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * 2);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(sampleCount * 2);
        }

        private static void WriteWavData(BinaryWriter writer, float[] samples)
        {
            foreach (float sample in samples)
            {
                short intSample = (short)(sample * short.MaxValue);
                writer.Write(intSample);
            }
        }
    }
}