using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    [Header("Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform qooboTransform;
    [SerializeField] private GameObject DonutBoxPrefab;
    [SerializeField] private MetricsMenuController metricsMenuController;
    [SerializeField] private EmotionModel emotionModel;
    [SerializeField] private EmotionController emotionController;
    private GameObject currentSpawnedDonutBox;
    
    private bool originalGazeToggle = true;
    private bool originalDistanceToggle = true;
    private bool originalSpeechToggle = true;

    private bool is_Training = false;

    public void StartTraining()
    {
        if (is_Training)
        {
            Deactivate();
            return;
        }
        else
        {
            is_Training = true;
        }

        if (showDebugLogs)
        {
            Debug.Log("Training started");
        }

        IsolateQooboEmotion();
        InstantiateDonutBox();
       
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
    private void IsolateQooboEmotion()//stops donuts and strokes from effecting qoobo so it can all be done manually from the WoZmanager
    {
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
        
    }
    private void UndoIsolateQooboEmotion()
    {
        if (emotionModel != null)
        {
            emotionModel.SetEventWeight(0.7f);
            emotionModel.SetMoodWeight(0.3f);
        }

        emotionController.TryDisplayEmotion("Neutral", "WoZ_Ladder_Override", true);

        metricsMenuController.SetGazeToggle(originalGazeToggle);
        metricsMenuController.SetDistanceToggle(originalDistanceToggle);
        metricsMenuController.SetSpeechToggle(originalSpeechToggle);
        
    }

    public void Deactivate()
    {
        if (!is_Training) return;
        UndoIsolateQooboEmotion();

        Destroy(currentSpawnedDonutBox);
        is_Training = false;

        if (showDebugLogs)
        {
            Debug.Log("Training stopped");
        }
    }

    public bool IsTraining()
    {
        return is_Training;
    }

}
