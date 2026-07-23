using UnityEngine;

public class WoZManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TrainController trainController;
    [SerializeField] private TrickLearner trickLearner;
    [SerializeField] private EmotionShowController emotionShowController;
    [SerializeField] private EmotionController emotionController;

    private int EmotionSelection = 1;//starts the emotion gloomy

    void Update()
    {
        TrainingRemoteInput();
        EmotionModelRemoteController();
        
    }

    private void TrainingRemoteInput()
    {
        if(!trainController.IsTraining()) {return;}

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))// A button Right
        {
            trickLearner.HandleRemoteInput(new string[] { "flip" });
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))// B button Right
        {
            trickLearner.HandleRemoteInput(new string[] { "flip", "flip" });
        }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))// X button Left
        {
            trickLearner.HandleRemoteInput(new string[] { "flip", "spin" });
        }

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))// Y button Left
        {
            trickLearner.HandleRemoteInput(new string[] { "flip", "spin", "roll" });
        }
    }

private void EmotionModelRemoteController()
{
    if(!emotionShowController.IsEmotionShowActive()) {return;}
  
    if (!emotionController.GetCanDisplayEmotion()){return;}
    
    // B button Right (Move up the ladder emotions get better)
    if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
    {
        EmotionSelection++;
        EmotionSelection = Mathf.Clamp(EmotionSelection, 1, 5); 
        emotionShowController.UpdateEmotionShow(EmotionSelection);
    }
    
    // A button Right (Move down the ladder emotions get worse)
    if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
    {
        EmotionSelection--;
        EmotionSelection = Mathf.Clamp(EmotionSelection, 1, 5);
        emotionShowController.UpdateEmotionShow(EmotionSelection);
    }
}




}


// if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
// {
//     Debug.Log("Right Joystick was clicked down!");
//     // Put your Qoobo command here
// }

// if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch))
// {
//     Debug.Log("Left Joystick was clicked down!");
//     // Put your Qoobo command here
// }