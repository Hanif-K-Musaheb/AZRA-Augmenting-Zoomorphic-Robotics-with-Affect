using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomisationMenuController : MonoBehaviour
{    
    [Header("Emotional State Buttons")]
    [SerializeField] private Button happyButton;
    [SerializeField] private Button neutralButton;
    [SerializeField] private Button annoyedButton;
    [SerializeField] private Button sadButton;

    [Header("Customisation Assets")]
    [SerializeField] private GameObject testCubeHat;
    
    
    void Start()
    {
        // Setup emotional state buttons
        SetupEmotionalStateButtons();
        
    }
    
    void SetupEmotionalStateButtons()
    {
        if (happyButton != null)
        {
            happyButton.onClick.AddListener(() => EquipHat("Cube"));
        }
        
        if (neutralButton != null)
        {
            neutralButton.onClick.AddListener(() => EquipHat("None"));
        }
        
        if (annoyedButton != null)
        {
            annoyedButton.onClick.AddListener(() => EquipHat("None"));
        }
        
        if (sadButton != null)
        {
            sadButton.onClick.AddListener(() => EquipHat("None"));
        }
    }

    void EquipHat(string hatType)
    {
        switch (hatType)
        {
            case "Cube":
                testCubeHat.SetActive(true); // Turns the cube on
                Debug.Log("CustomisationMenu: Equipped Cube Hat!");
                break;

            case "None":
                testCubeHat.SetActive(false); // Turns the cube off
                Debug.Log("CustomisationMenu: Removed all hats.");
                break;
        }
    }
    
    void SetEmotionalState(string state)
    {
        switch (state)
        {
            case "Happy":
                //emotionModel.SetHappyState();
                Debug.Log("MetricsMenu: Set robot to Happy state (V: 6, A: 0)");
                break;
            case "Neutral":
                //emotionModel.SetNeutralState();
                Debug.Log("MetricsMenu: Set robot to Neutral state (V: 0, A: 0)");
                break;
            case "Annoyed":
                //emotionModel.SetAnnoyedState();
                Debug.Log("MetricsMenu: Set robot to Annoyed state (V: -6, A: 6)");
                break;
            case "Sad":
                //emotionModel.SetSadState();
                Debug.Log("MetricsMenu: Set robot to Sad state (V: -6, A: 0)");
                break;
        }
    }
}
   

    
  

    
 
    
