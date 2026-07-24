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
    public SimpleToggle CustomisationWindowToggle;


    private float nextAllowedClickTime=0;
    private float globalCooldown = 0;
    private string currentFeature;

    public void OnSelectTrainingFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        trainController.StartTraining(); 
        currentFeature="train";
   
    }

    
    public void OnSelectEmotionShowFeature()
    {

        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        emotionShowController.StartEmotionShow();
        currentFeature="emotion show"; 
       
    }

    public void OnSelectTVFeature()
    {

        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        TVcontroller.ToggleObject();
        currentFeature="TV";
      
    }

    public void OnSelectFrisbeeFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();
        
        frisbeeController.OnSpawnButtonClicked();
        currentFeature="frisbee";
      
        
    }

    public void OnSelectCustimisationFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();

        CustomisationWindowToggle.ActivateCusomistionWindow();
        currentFeature="customisation";
    }

    private void TurnOffAllFeatures()
    {
        currentFeature=null;
        if (trainController != null) trainController.Deactivate();
        if (emotionShowController != null) emotionShowController.Deactivate();
        if (TVcontroller != null) TVcontroller.Deactivate();
        if (frisbeeController != null) frisbeeController.Deactivate();
        if (CustomisationWindowToggle !=null) CustomisationWindowToggle.Deactivate();
        
        // Add other controllers here as you build them
    }

    public string GetCurrentFeature()
    {
        return currentFeature;
    }

 
}
