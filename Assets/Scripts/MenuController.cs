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
    public FeatureSignController signController;
    [Header("Feature Colors")]
    public Color trainColor = Color.white;
    public Color emotionColor = Color.white;
    public Color tvColor = Color.white;
    public Color frisbeeColor = Color.white;
    public Color customColor = Color.white;
   


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
        signController.ShowExplaination("Training",
                    "Teach Qoobo tricks by saying the command, then rewarding it with praise, a donut, or a stroke!\n\nTricks to teach:\n1. Flip\n2. Double Flip\n3. Flip + Spin\n4. Flip + Spin + Roll",
                    trainColor);   
    }

    
    public void OnSelectEmotionShowFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        emotionShowController.StartEmotionShow();
        currentFeature="emotion show"; 
        signController.ShowExplaination("Emotion Model",
                    "In this feature Qoobo will start in a negative state and you must interact with Qoobo to change its mood.",
                    emotionColor);   
    }

    public void OnSelectTVFeature()
    {

        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        TVcontroller.ToggleObject();
        currentFeature="TV";
        signController.ShowExplaination("TV",
                    "In this just sit back and relax with Qoobo however you may like",
                    tvColor);  

      
    }

    public void OnSelectFrisbeeFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();
        
        frisbeeController.OnSpawnButtonClicked();
        currentFeature="frisbee";
        signController.ShowExplaination("Catch with a Frisbee",
                        "Throw the frisbee and say 'catch'\n When Qoobo picks up the frisbee say “retrieve”.\n When Qoobo is next to you with the frisbee say 'drop it'",
                        frisbeeColor);
    }

    public void OnSelectCustimisationFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();

        CustomisationWindowToggle.ActivateCusomistionWindow();
        currentFeature="customisation";
        signController.ShowExplaination("Customise Qoobo's Head wear",
                        "Customise Qoobo’s head wear until you and Qoobo have come to a decision you can both agree on.",
                        customColor);
    
    }

    private void TurnOffAllFeatures()
    {
        currentFeature=null;
        signController.HideSign();
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
