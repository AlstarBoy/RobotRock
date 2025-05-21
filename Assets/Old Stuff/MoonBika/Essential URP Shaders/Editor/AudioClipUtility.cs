using UnityEditor;
using UnityEngine;
using System.IO;

namespace MoonBika
{
    public static class AudioClipUtility
    {
        public static void SaveClip(AudioClip clip, string fileName)
        {
            string path = "Assets/GeneratedAudio/" + fileName + ".wav";
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