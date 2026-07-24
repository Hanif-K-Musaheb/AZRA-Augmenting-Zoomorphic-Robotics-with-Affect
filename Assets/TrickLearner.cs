using UnityEngine;
using LLMUnity; 
using System.Collections;

public class TrickLearner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GhostModeController ghostModeController; 
    
    [Header("Debug Settings")]
    [SerializeField] private bool showDebugLogs = true;

    [Header("LLM Settings")]
    public LLMAgent llmAgent; 
    [SerializeField] private int maxMovesInSequence = 15; // Limit the number of moves in a single trick sequence for the LLM

    
    // The data container that Unity will convert the AI's JSON into
    
    public class QooboMoveSet
    {
        public string name;
        public string[] move_set;
    }

    // Call this method whenever Vosk finishes transcribing a sentence
    public async void ParseVoskSpeech(string voskTranscription)
    {
        if(showDebugLogs)
            Debug.Log("Asking the AI to parse the trick...");

        // this clears the chat history so the AI doesn't remember between calls, pehaps i need to change this so that it ca be better at learning over time
        // but for now we will clear it each time for testing purposes.
        await llmAgent.ClearHistory();

        // We send the text to the LLMAgent and wait for it to generate the JSON text
        string jsonResponse = await llmAgent.Chat(voskTranscription);
        
        if(showDebugLogs)
            Debug.Log("AI Output: " + jsonResponse);

        // Convert the raw JSON text string into the C# QooboMoveSet object
        try
        {
            QooboMoveSet newTrick = JsonUtility.FromJson<QooboMoveSet>(jsonResponse);
            
            Debug.Log($"Success! Qoobo learned '{newTrick.name}'.");
            Debug.Log($"The first move to execute is: {newTrick.move_set[0]}");
            
            StartCoroutine(HandleTrickJSON(newTrick.move_set));
        }
        catch (System.Exception e)
        {
            Debug.LogError("The AI failed to output valid JSON: " + e.Message);
        }
    }
    private IEnumerator HandleTrickJSON(string[] moveSet)
    {
        if (showDebugLogs)
            {Debug.Log("HandleTrickJSON called. This is where you would parse the JSON and trigger the appropriate trick.");
            Debug.Log($"moveSet: {string.Join(", ", moveSet)}, length: {moveSet.Length}");}

        if (moveSet == null || moveSet.Length == 0 || moveSet.Length > maxMovesInSequence)
        {
            if (showDebugLogs)
                Debug.LogWarning("Received empty or invalid move set data.");
            yield break; 
        }

        ghostModeController.ToggleGhostMode();

        yield return new WaitForSeconds(ghostModeController.GetTransitionDuration()); //so it can go into ghost mode before starting the trick

        foreach (string move in moveSet)
        {
            switch (move.ToLower())
            {
                case "jump":
                    ghostModeController.TriggerJump();
                    break;
                case "spin":
                    ghostModeController.TriggerSpin();
                    break;
                case "roll":
                    ghostModeController.TriggerRoll();
                    break;
                case "flip":
                    ghostModeController.TriggerFlip();
                    break;
                default:
                    if (showDebugLogs)
                        Debug.LogWarning($"Unknown move: '{move}' ");
                    break;
            }
            yield return new WaitForSeconds(ghostModeController.GetTrickDuration());
        }
        ghostModeController.ToggleGhostMode();
    }

    public void HandleRemoteInput(string[] moveSet)
    {
        StartCoroutine(HandleTrickJSON(moveSet));
    }
}