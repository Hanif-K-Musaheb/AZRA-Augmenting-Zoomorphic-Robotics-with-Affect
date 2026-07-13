using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomisationMenuController : MonoBehaviour
{    
    [SerializeField] private bool showDebugLogs = false;  // Debug log toggle
    
    [Header("Emotional State Buttons")]
    [SerializeField] private Button CowboyHatButton;
    [SerializeField] private Button MagicianHatButton;
    [SerializeField] private Button SombreroHatButton;
    [SerializeField] private Button NoHatButton;

    [Header("Customisation Assets")]
    [SerializeField] private GameObject CowboyHat;
    [SerializeField] private GameObject MagicianHat;
    [SerializeField] private GameObject SombreroHat;

    [Header("Emotion Model and Controller")]    
    [SerializeField] private EmotionModel emotionModel;
    [SerializeField] private EmotionController emotionController;
    private float emotionDisplayCooldown;
    private float timeSinceLastClick;
    private int HatSelections = 0;
    private Button[] allHatButtons;

    
    
    void Start()
    {
        // Setup emotional state buttons
        SetupEmotionalStateButtons();
        //emotionDisplayCooldown = emotionController.GetDisplayCoolDown()+2;
        timeSinceLastClick = 0f;
        allHatButtons = new Button[] { CowboyHatButton, MagicianHatButton, SombreroHatButton, NoHatButton };
    }
    void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        if (emotionController.GetCanDisplayEmotion())
        {
            SetButtonsInteractable(true);
        }
    }
    
    void SetupEmotionalStateButtons()
    {
        if (CowboyHatButton != null)
        {
            CowboyHatButton.onClick.AddListener(() => EquipHat("Cowboy"));
        }

        if (MagicianHatButton != null)
        {
            MagicianHatButton.onClick.AddListener(() => EquipHat("Magician"));
        }

        if (SombreroHatButton != null)
        {
            SombreroHatButton.onClick.AddListener(() => EquipHat("Sombrero"));
        }

        if (NoHatButton != null)
        {
            NoHatButton.onClick.AddListener(() => EquipHat("None"));
        }
    }

    void EquipHat(string hatType)
    {
        if (showDebugLogs)
            Debug.Log($"CustomisationMenu: Equipping hat type: {hatType}");

        switch (hatType)
        {
            case "Cowboy":
                MagicianHat.SetActive(false);
                SombreroHat.SetActive(false);
                CowboyHat.SetActive(true); // Turns the cowboy hat on
                if (showDebugLogs)
                    Debug.Log("CustomisationMenu: Equipped Cowboy Hat!");
                HatSelections++;
                ResponseToHatSelection(HatSelections);
                break;
            case "Magician":
                CowboyHat.SetActive(false);
                SombreroHat.SetActive(false);
                MagicianHat.SetActive(true); // Turns the magician hat on
                if (showDebugLogs)
                    Debug.Log("CustomisationMenu: Equipped Magician Hat!");
                HatSelections++;
                ResponseToHatSelection(HatSelections);
                break;
            case "Sombrero":
                CowboyHat.SetActive(false); 
                MagicianHat.SetActive(false); 
                SombreroHat.SetActive(true); // Turns the sombrero hat on
                if (showDebugLogs)
                    Debug.Log("CustomisationMenu: Equipped Sombrero Hat!");
                HatSelections++;
                ResponseToHatSelection(HatSelections);
                break;

            case "None":
                CowboyHat.SetActive(false); // Turns the cowboy hat off
                MagicianHat.SetActive(false); // Turns the magician hat off
                SombreroHat.SetActive(false); // Turns the sombrero hat off
                if (showDebugLogs)
                    Debug.Log("CustomisationMenu: Removed all hats.");
                HatSelections++;
                ResponseToHatSelection(HatSelections);
                break;
        }
    }

    void ResponseToHatSelection(int hatSelections)
    {
        if (showDebugLogs)
            Debug.Log($"CustomisationMenu: HatSelections value: {HatSelections}");

        if (emotionModel == null || emotionController == null)
        {
            Debug.LogError("CustomisationMenu: EmotionModel or EmotionController is not assigned.");
            return;
        }

        if (emotionController.GetCanDisplayEmotion() == false)
        {
            if (showDebugLogs)
                Debug.Log("CustomisationMenu: Emotion display is on cooldown. Please wait.");
            return;
        }
        else
        {
            timeSinceLastClick = 0f; // Reset the cooldown timer
            SetButtonsInteractable(false); // Grey out buttons until cooldown ends
        }

        switch (HatSelections)
        {
            case 1:
                if (showDebugLogs)
                    Debug.Log("Detected: An angry word!==========>1");
                var response = emotionModel.CalculateEmotionalResponse("AngryHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                break;
            case 2:
                if (showDebugLogs)
                    Debug.Log("Detected: A sad word!==========>2");
                response = emotionModel.CalculateEmotionalResponse("SadHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                break;
            case 3:
                if (showDebugLogs)
                    Debug.Log("Detected: A happy word!==========>3  ");
                response = emotionModel.CalculateEmotionalResponse("HappyHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                break;
            default:
            if (showDebugLogs)
                    Debug.Log("Detected: Words of praise!==========>4");
                response = emotionModel.CalculateEmotionalResponse("PraiseHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                HatSelections = Random.Range(0, 4); // Reset to a random value between 0 and 3, this is if they keep clicking different hats
                break;
        }
    }
    void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in allHatButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }
    

}
   

    
  

    
 
    
