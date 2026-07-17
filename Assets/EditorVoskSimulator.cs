using UnityEngine;
using UnityEngine.InputSystem; // Added to access the modern Input System

public class EditorVoskSimulator : MonoBehaviour
{
    public TrickLearner trickLearner; 
    //example input:
    //"hey qoobo spin around twice and then jump and we will call that the happy dance"

    private string fakeSpeechInput ="Qoobo, please spin three times and then jump three times which is what I call excercise";

    void Update()
    {
        // Using the New Input System to check for the Spacebar
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("Simulating Vosk hearing: " + fakeSpeechInput);
            trickLearner.ParseVoskSpeech(fakeSpeechInput);
        }
    }
}