using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Feature Controllers")]
    public TrainController trainController;
    public EmotionShowController emotionShowController;
    public ObjectToggler TVcontroller;
    public FrisbeeController frisbeeController;


    private float nextAllowedClickTime=0;
    private float globalCooldown = 0;

    public void OnSelectTrainingFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        trainController.StartTraining(); 
        Debug.Log("Switched to Training Mode.");
    }

    
    public void OnSelectEmotionShowFeature()
    {

        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        emotionShowController.StartEmotionShow(); 
        Debug.Log("Switched to Emotion Show Mode.");
    }

    public void OnSelectTVFeature()
    {

        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        TVcontroller.ToggleObject();
        Debug.Log("Switched to Emotion Show Mode.");
    }

    public void OnSelectFrisbeeFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();
        
        frisbeeController.OnSpawnButtonClicked();
        Debug.Log("Switched to Emotion Show Mode.");
        
    }



    
    private void TurnOffAllFeatures()
    {
        if (trainController != null) trainController.Deactivate();
        if (emotionShowController != null) emotionShowController.Deactivate();
        if (TVcontroller != null) TVcontroller.Deactivate();
        if (frisbeeController != null) frisbeeController.Deactivate();
        
        // Add other controllers here as you build them
    }

 
}
