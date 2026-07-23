using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionShowController : MonoBehaviour
{

    [Header("Emotion Model and Controller")]    
    [SerializeField] private EmotionModel emotionModel;
    [SerializeField] private EmotionController emotionController;
    [Header("Debug")]  
    [SerializeField] private bool showDebugLogs =false; 

    public float cooldownTime = 2.0f;
    private float nextAllowedClickTime = 0f;
    private bool isEmotionShowActive = false;

    public void StartEmotionShow()
    {
        // if (Time.time < nextAllowedClickTime)//stops issue with AR double click
        // {
        //     Debug.Log("Button is on cooldown, Ignoring click.");
        //     return; 
        // }
        // nextAllowedClickTime = Time.time + cooldownTime;

        if (isEmotionShowActive)
        {
            Deactivate();
            return;
        }
        else
        {
            isEmotionShowActive = true;
        }

        if (showDebugLogs)
        {
            Debug.Log("Emotion Show started<---------------------------------");
        }
    }

    public bool IsEmotionShowActive()
    {
        return isEmotionShowActive;
    }


    public void UpdateEmotionShow(int EmotionSelection)
    {
        string targetEmotion = "Neutral"; 

        switch (EmotionSelection)
        {
            case 1:
                targetEmotion = "Sad";
                break;
            case 2:
                targetEmotion = "Gloomy";
                break;
            case 3:
                targetEmotion = "Neutral";
                break;
            case 4:
                targetEmotion = "Happy";
                break;
            case 5:
                targetEmotion = "Excited";
                break;
        }

        if (showDebugLogs)
        {
            Debug.Log($"WoZ Override: Emotion ladder moved to Level {EmotionSelection} ({targetEmotion})");
        }


        emotionController.TryDisplayEmotion(targetEmotion, "WoZ_Ladder_Override");
    }

    public void Deactivate()
    {
        if(!isEmotionShowActive)return;

        isEmotionShowActive = false;
        emotionController.TryDisplayEmotion("Neutral", "WoZ_Ladder_Override");

        if (showDebugLogs)
        {
            Debug.Log("Emotion Show has stopped");
        }
    }
  
}

