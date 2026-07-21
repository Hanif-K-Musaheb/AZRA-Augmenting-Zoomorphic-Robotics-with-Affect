using UnityEngine;
using UnityEngine.InputSystem; // Added to access the modern Input System

public class EditorVoskSimulator : MonoBehaviour
{
    public TrickLearner trickLearner; 
    //example inputs:
    //"hey qoobo spin around twice and then jump and we will call that the happy dance"
    //"Qoobo, please spin three times and then jump three times which is what I call excercise";

    private string fakeSpeechInput = "hey qoobo can you do two rolls for me then a flip";

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("Simulating Vosk hearing: " + fakeSpeechInput);
            trickLearner.ParseVoskSpeech(fakeSpeechInput);
        }
    }
}