using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StudyAudionRecorder : MonoBehaviour
{
   private string micDevice;
    private AudioClip recordingClip;
    private bool isRecording = false;

    // Set to the MAXIMUM possible length of a study session. 
    // 3600 seconds = 1 hour. Don't worry, it trims the empty space at the end when saving!
    public int maxRecordingTimeSeconds = 3600; 

    void Start()
    {
        // Grab the Quest 3's default microphone
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            StartStudyRecording();
        }
        else
        {
            Debug.LogError("Qoobo Study: No microphone found!");
        }
    }

    private void StartStudyRecording()
    {
        if (micDevice == null) return;
        
        // Start recording immediately. 'false' means it will NOT loop over and overwrite itself.
        recordingClip = Microphone.Start(micDevice, false, maxRecordingTimeSeconds, 44100);
        isRecording = true;
        Debug.Log("Qoobo Study: Voice recording started automatically on load...");
    }

    // This built-in Unity method fires automatically when you close the app via the Quest Menu
    void OnApplicationQuit()
    {
        StopAndSaveRecording();
    }

    // Fallback: If the script is ever destroyed during gameplay, save the file just in case.
    void OnDestroy()
    {
        StopAndSaveRecording();
    }

    private void StopAndSaveRecording()
    {
        if (!isRecording) return;
        
        int position = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        isRecording = false;
        
        if (recordingClip == null || position == 0) return;

        // 1. Trim the empty space off the end of the recording
        float[] soundData = new float[position * recordingClip.channels];
        recordingClip.GetData(soundData, 0);
        AudioClip trimmedClip = AudioClip.Create("StudyRecord", position, recordingClip.channels, recordingClip.frequency, false);
        trimmedClip.SetData(soundData, 0);

        // 2. Save it to the Quest's internal storage
        SaveAsWav(trimmedClip);
    }

    private void SaveAsWav(AudioClip clip)
    {
        // Generates a unique name like: ParticipantVoice_20260731_1130.wav
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        string filepath = Path.Combine(Application.persistentDataPath, $"ParticipantVoice_{timestamp}.wav");

        using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fileStream))
        {
            int sampleCount = clip.samples * clip.channels;
            int frequency = clip.frequency;
            
            // Write standard WAV Header
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(36 + sampleCount * 2);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)clip.channels);
            writer.Write(frequency);
            writer.Write(frequency * clip.channels * 2);
            writer.Write((short)(clip.channels * 2));
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(sampleCount * 2);

            // Convert float audio data to 16-bit PCM
            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);
            foreach (float sample in samples)
            {
                int intData = (int)(sample * 32767f);
                if (intData > 32767) intData = 32767;
                if (intData < -32768) intData = -32768;
                writer.Write((short)intData);
            }
        }
        Debug.Log($"Qoobo Study: Audio saved successfully to {filepath}");
    }
}
