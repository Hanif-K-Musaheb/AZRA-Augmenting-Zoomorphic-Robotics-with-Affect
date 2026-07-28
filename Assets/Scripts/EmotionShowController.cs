using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionShowController : MonoBehaviour
{
    [Header("Emotion Model and Controller")]    
    [SerializeField] private EmotionModel emotionModel;
    [SerializeField] private EmotionController emotionController;
    [SerializeField] private MetricsMenuController metricsMenuController;
    [SerializeField] private Transform qooboTransform;
    [SerializeField] private GameObject DonutBoxPrefab;
    
    [Header("Debug")]  
    [SerializeField] private bool showDebugLogs = true; 

    private bool isEmotionShowActive = false;
    private int currentLadderLevel = 1; // 1 = Sad, 5 = Excited
    
    // Store original interaction states to restore after emotion show
    private bool originalGazeToggle = true;
    private bool originalDistanceToggle = true;
    private bool originalSpeechToggle = true;

    private GameObject currentSpawnedDonutBox;

    public void StartEmotionShow()
    {
        isEmotionShowActive = true;
        currentLadderLevel = 1; // Always reset to Sad when the feature starts

         if (metricsMenuController != null)
        {
            // Disable gaze, distance, and speech toggles during tour
            metricsMenuController.SetGazeToggle(false);
            metricsMenuController.SetDistanceToggle(false);
            metricsMenuController.SetSpeechToggle(false);
        }

        // Disable standard event reactions so touch/food don't auto-change mood
        if (emotionModel != null)
        {
            emotionModel.SetEventWeight(0f);
            emotionModel.SetMoodWeight(1f);
        }

        if (showDebugLogs)
        {
            Debug.Log("Emotion Show started - Forcing Sad state");
        }

        UpdateEmotionDisplay();

        InstantiateDonutBox();
    }

    public bool IsEmotionShowActive()
    {
        return isEmotionShowActive;
    }

    // Called by the WoZManager (B button)
    public void StepUp()
    {
        if (!isEmotionShowActive) return;
        currentLadderLevel = Mathf.Clamp(currentLadderLevel + 1, 1, 5);
        UpdateEmotionDisplay();
    }

    // Called by the WoZManager (A button)
    public void StepDown()
    {
        if (!isEmotionShowActive) return;
        currentLadderLevel = Mathf.Clamp(currentLadderLevel - 1, 1, 5);
        UpdateEmotionDisplay();
    }

    private void UpdateEmotionDisplay()
    {
        string targetEmotion = "Neutral"; 

        // Change the underlying mood so the passive face matches the ladder
        switch (currentLadderLevel)
        {
            case 1:
                targetEmotion = "Sad";
                emotionModel.SetSadState(); 
                break;
            case 2:
                targetEmotion = "Gloomy";
                emotionModel.SetEmotionalState(-8f, -8f);
                break;
            case 3:
                targetEmotion = "Neutral";
                emotionModel.SetNeutralState();
                break;
            case 4:
                targetEmotion = "Happy";
                emotionModel.SetHappyState();
                break;
            case 5:
                targetEmotion = "Excited";
                emotionModel.SetEmotionalState(8f, 8f);
                break;
        }

        if (showDebugLogs)
        {
            Debug.Log($"WoZ Override: Emotion ladder moved to Level {currentLadderLevel} ({targetEmotion})");
        }

        // Trigger the display immediately, bypassing standard cooldowns
        emotionController.TryDisplayEmotion(targetEmotion, "WoZ_Ladder_Override", true);
    }

    public void Deactivate()
    {
        if (!isEmotionShowActive) return;

        isEmotionShowActive = false;

        // Restore normal interaction weights when you switch to a different feature
        if (emotionModel != null)
        {
            emotionModel.SetEventWeight(0.7f);
            emotionModel.SetMoodWeight(0.3f);
        }

        emotionController.TryDisplayEmotion("Neutral", "WoZ_Ladder_Override", true);

        if (showDebugLogs)
        {
            Debug.Log("Emotion Show has stopped");
        }

        metricsMenuController.SetGazeToggle(originalGazeToggle);
        metricsMenuController.SetDistanceToggle(originalDistanceToggle);
        metricsMenuController.SetSpeechToggle(originalSpeechToggle);

        if (currentSpawnedDonutBox != null)
        {
            Destroy(currentSpawnedDonutBox);
        }

    }

    private void InstantiateDonutBox()
    {
        if (currentSpawnedDonutBox != null)
        {
            Destroy(currentSpawnedDonutBox);
        }

        if (qooboTransform == null)return;
        
        float rightOffset = -0.6f;   
        float forwardOffset = 0.1f; 

        //calculate the position of the donut box
        Vector3 calculatedSpawnPosition = qooboTransform.position 
                                        + (qooboTransform.forward * forwardOffset)
                                        + (qooboTransform.right * rightOffset);

        calculatedSpawnPosition.y = qooboTransform.position.y;//force it to be on the table with Qoobo          
        Quaternion rotationOffset = Quaternion.Euler(0, -135f, 0); 
        Quaternion finalRotation = qooboTransform.rotation * rotationOffset; 

        currentSpawnedDonutBox = Instantiate(DonutBoxPrefab, calculatedSpawnPosition, finalRotation);

        if (showDebugLogs)
        {
            Debug.Log("Prefab spawned!");
        }
        
    }
}