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
    public FeatureSignController signController;
    public HatManager CustomisationManager;
    [Header("Feature Colors")]
    public Color trainColor = Color.white;
    public Color emotionColor = Color.white;
    public Color tvColor = Color.white;
    public Color frisbeeColor = Color.white;
    public Color customColor = Color.white;
   


    private float nextAllowedClickTime=0;
    private float globalCooldown = 1;
    private string currentFeature;
    private bool TVon = false;

    public void OnSelectTrainingFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;

        TurnOffAllFeatures();

        trainController.StartTraining(); 
        currentFeature="train";
        signController.ShowExplaination("Training",
                    "Teach Qoobo tricks by saying the command, then rewarding it with praise, a donut, or a stroke!\n\nTricks to teach:\n1. Flip\n2. Double Flip\n3. Flip + Spin\n4. Flip + Spin + Roll\nNote: Qoobo wont always be obedient",
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
        //More complicated here because the TV button needed to be able to deactivate when moving to another feature and when pressing the TV button again
        if (Time.time < nextAllowedClickTime) 
        {
            return; 
        }
        nextAllowedClickTime = Time.time + globalCooldown;

        if (currentFeature == "TV")
        {
            TurnOffAllFeatures();
            return; 
        }
        TurnOffAllFeatures();

        TVcontroller.ToggleObject(); 
        currentFeature = "TV";
        
        if (signController != null)
        {
            signController.ShowExplaination("TV", "In this just sit back and relax with Qoobo however you may like", tvColor);  
        }
        else
        {
            Debug.LogWarning("SignController is missing in the Inspector! Skipping sign.");
        }
        
    }

    public void OnSelectFrisbeeFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();
        
        frisbeeController.OnSpawnButtonClicked();
        currentFeature="frisbee";
        signController.ShowExplaination("Catch with a Frisbee",
                        "Throw the frisbee and say 'catch'\n Say “retrieve” for Qoobo to pick up the frisbee.\n When Qoobo is next to you with the frisbee say 'drop it' and firsbee will disapear\n To play again simply press the frisbee button",
                        frisbeeColor);
    }

    public void OnSelectCustimisationFeature()
    {
        if (Time.time < nextAllowedClickTime) return; 
        nextAllowedClickTime = Time.time + globalCooldown;
        TurnOffAllFeatures();

        // CustomisationWindowToggle.ActivateCusomistionWindow();
        CustomisationManager.ActivateHats();
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
        if(CustomisationManager!=null)CustomisationManager.Deactivate();
        
        //when adding features to the menu controller add there deactivation here
    }

    public string GetCurrentFeature()
    {
        return currentFeature;
    }
}
