using UnityEngine;
using LLMUnity; 

public class TrickLearner : MonoBehaviour
{
    [Header("LLM Settings")]
    public LLMAgent llmAgent; // Drag your Qoobo_Brain (which has the LLMAgent) here in the Inspector
    
    // The data container that Unity will convert the AI's JSON into
    [System.Serializable]
    public class QooboMoveSet
    {
        public string name;
        public string[] move_set;
    }

    // Call this method whenever Vosk finishes transcribing a sentence
    public async void ParseVoskSpeech(string voskTranscription)
    {
        Debug.Log("Asking the AI to parse the trick...");

        // FIX 1: Wipe the AI's memory so it treats every command as a brand new task <-- might need to get rid of this if you want the AI to remember previous commands and build on them
        await llmAgent.ClearHistory();

        // We send the text to the LLMAgent and wait for it to generate the JSON text
        string jsonResponse = await llmAgent.Chat(voskTranscription);
        
        Debug.Log("AI Output: " + jsonResponse);

        // Convert the raw JSON text string into the C# QooboMoveSet object
        try
        {
            QooboMoveSet newTrick = JsonUtility.FromJson<QooboMoveSet>(jsonResponse);
            
            Debug.Log($"Success! Qoobo learned '{newTrick.name}'.");
            Debug.Log($"The first move to execute is: {newTrick.move_set[0]}");
            
            // This is where you will eventually send the move_set array to your animation controller!
        }
        catch (System.Exception e)
        {
            Debug.LogError("The AI failed to output valid JSON: " + e.Message);
        }
    }
}