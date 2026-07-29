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
    //fetch variables
    // public enum FetchState { Idle, ChasingFrisbee, ReturningToPlayer }

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
        
        // This directly matches how your MenuController sets the string
        if(currentFeature != "emotion show") { return; }
    
        // B button Right (Move up the ladder, emotions get better)
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            emotionShowController.StepUp();
        }
        
        // A button Right (Move down the ladder, emotions get worse)
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
        // A button Right (Move down the ladder, emotions get worse)
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log(ghostModeController.DebugStatement()); 
            // Use GhostModeController.FetchState instead of just FetchState
            GhostModeController.FetchState currentFetchState = ghostModeController.GetFetchState();

            switch (currentFetchState) // Make sure to switch on 'currentFetchState', not 'fetchState'
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
    private void ManualReactionController()
    {
        // We only want these active during the emotion show
        string currentFeature = menuController.GetCurrentFeature();
        if (currentFeature != "emotion show" && currentFeature != "train") { return; }

        // Right Grip Trigger: Small Positive Reaction
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            emotionController.TryDisplayEmotion("Happy", "WoZ_Small_Positive", true);
            Debug.Log("WoZ: Triggered Small Positive Reaction (Right Grip)");
        }

        // Left Grip Trigger: Small Negative Reaction
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            emotionController.TryDisplayEmotion("Sad", "WoZ_Small_Negative", true);
            Debug.Log("WoZ: Triggered Small Negative Reaction (Left Grip)");
        }
    }


}