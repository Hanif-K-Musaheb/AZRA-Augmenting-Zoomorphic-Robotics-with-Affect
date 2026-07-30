using UnityEngine;

public class WoZManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TrainController trainController;
    [SerializeField] private TrickLearner trickLearner;
    [SerializeField] private EmotionShowController emotionShowController;
    [SerializeField] private EmotionController emotionController;
    [SerializeField] private EndOfFeatureQuesitonaire endOfFeatureQuesitonaire;
    [SerializeField] private MenuController menuController;
    [SerializeField] private GhostModeController ghostModeController;

    void Update()
    {
        TrainingRemoteInput();
        EmotionModelRemoteController();
        FrisbeeRemoteController();
        TVRemoteController();
        CustomisationRemoteController();
        ManualReactionController();

    }
    
    private void TrainingRemoteInput()
    {
        string currentFeature = menuController.GetCurrentFeature();
        if(currentFeature != "train") { return; }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)) // A button Right
        {
            trickLearner.HandleRemoteInput(new string[] { "flip" });
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)) // B button Right
        {
            trickLearner.HandleRemoteInput(new string[] { "flip", "flip" });
        }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)) // X button Left
        {
            trickLearner.HandleRemoteInput(new string[] { "flip", "spin" });
        }

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch)) // Y button Left
        {
            trickLearner.HandleRemoteInput(new string[] { "flip", "spin", "roll" });
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            endOfFeatureQuesitonaire.StartQuestionnaire(currentFeature);
        }
    }
    
    private void EmotionModelRemoteController()
    {
        string currentFeature = menuController.GetCurrentFeature();
        
        if(currentFeature != "emotion show") { return; }
    
        // B button (left) moves up the ladder of emotion 
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            emotionShowController.StepUp();
        }
        
        // A Button (right) moves down the ladder of emotion
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            emotionShowController.StepDown();
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            endOfFeatureQuesitonaire.StartQuestionnaire(currentFeature);
        }
    }
    
    private void FrisbeeRemoteController()
    {
        string currentFeature = menuController.GetCurrentFeature();
        

        if(currentFeature != "frisbee") { return; }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            endOfFeatureQuesitonaire.StartQuestionnaire(currentFeature);
        }

       // A button forces the fetch sequence to move incase it gets stuck in the study
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log(ghostModeController.DebugStatement()); 
            GhostModeController.FetchState currentFetchState = ghostModeController.GetFetchState();

            switch (currentFetchState) 
            {
                case GhostModeController.FetchState.Idle:
                    ghostModeController.AllowFetch();
                    break; // Don't forget your break statements!

                case GhostModeController.FetchState.ChasingFrisbee:
                    ghostModeController.ForceRetrieve();
                    break;
                case GhostModeController.FetchState.ReturningToPlayer:
                    ghostModeController.AllowDrop();
                    break;
            }
        }
    }
    
    private void TVRemoteController()
    {
        string currentFeature = menuController.GetCurrentFeature();
        if(currentFeature != "TV") { return; }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            endOfFeatureQuesitonaire.StartQuestionnaire(currentFeature);
        }
    }

    private void CustomisationRemoteController()
    {
        string currentFeature = menuController.GetCurrentFeature();
        if(currentFeature != "customisation") { return; }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            endOfFeatureQuesitonaire.StartQuestionnaire(currentFeature);
        }
    }

    //Small reactions used so that the study doesn't have to rely on unreliable vosk translation
    private void ManualReactionController()
    {
        
        string currentFeature = menuController.GetCurrentFeature();
        if (currentFeature != "emotion show" && currentFeature != "train") { return; }//only active during emotion show and training

        // right grip trigger -> small positive reaction
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            emotionController.TryDisplayEmotion("Happy", "WoZ_Small_Positive", true);
            Debug.Log("WoZ: Triggered Small Positive Reaction (Right Grip)");
        }

        // right grip trigger -> small negative reaction
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            emotionController.TryDisplayEmotion("Sad", "WoZ_Small_Negative", true);
            Debug.Log("WoZ: Triggered Small Negative Reaction (Left Grip)");
        }
    }


}