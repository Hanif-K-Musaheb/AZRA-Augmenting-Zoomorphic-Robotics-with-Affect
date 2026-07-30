using UnityEngine;

public class HatManager : MonoBehaviour
{
    public static HatManager Instance { get; private set; }
    [SerializeField] private bool showDebugLogs = false;

    [Header("References")]
    [SerializeField] private EmotionModel emotionModel;
    [SerializeField] private EmotionController emotionController;
    [SerializeField] private GameObject[] allHats; 

    public HatInteractive currentlyEquippedHat;
    public bool isCustomisationMenuOpen = false;

    private int EquipCounter=0;//an on going counter as to how many times a person has a equiped a different hat so they user get different responses from Qoobo

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEquippedHat(HatInteractive newHat)
    {
        currentlyEquippedHat = newHat;
        EquipCounter++;

        EquipedHatEmotionalResponse(EquipCounter);

    }

    public void EquipedHatEmotionalResponse(int EmotionalResponseCode)
    {
        
        if (showDebugLogs)
            Debug.Log($"CustomisationMenu: HatSelections value: {EmotionalResponseCode}");

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
  

        switch (EmotionalResponseCode)
        {
            case 1:
                if (showDebugLogs)
                    Debug.Log("Detected: An angry word!");
                var response = emotionModel.CalculateEmotionalResponse("AngryHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                break;
            case 2:
                if (showDebugLogs)
                    Debug.Log("Detected: A sad word!");
                response = emotionModel.CalculateEmotionalResponse("SadHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                break;
            case 3:
                if (showDebugLogs)
                    Debug.Log("Detected: A happy word!");
                response = emotionModel.CalculateEmotionalResponse("HappyHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                break;
            default:
            if (showDebugLogs)
                    Debug.Log("Detected: Words of praise!");
                response = emotionModel.CalculateEmotionalResponse("PraiseHeard");
                emotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);
                EmotionalResponseCode = Random.Range(0, 4); // Reset to a random value between 0 and 3, this is if they keep clicking different hats
                break;
        }

    }

    public void ClearEquippedHat(HatInteractive removedHat)
    {
        if (currentlyEquippedHat == removedHat)
        {
            currentlyEquippedHat = null;
        }
    }

   
    public void ActivateHats()
    {
        isCustomisationMenuOpen = true;

        // 1. UN-PARENT the equipped hat so the user can easily grab it again!
        if (currentlyEquippedHat != null)
        {
            Collider[] colliders = currentlyEquippedHat.GetComponentsInChildren<Collider>();
            foreach(Collider col in colliders)
            {
                col.enabled = true;
            }
            currentlyEquippedHat.RemoveParenting();
        }

        foreach (GameObject hatObj in allHats)
        {
            if (hatObj != null)
            {
                hatObj.SetActive(true);

                Collider col = hatObj.GetComponentInChildren<Collider>();
                if (col != null) col.enabled = true;

                HatInteractive hatScript = hatObj.GetComponentInChildren<HatInteractive>();
                if (hatScript != null && hatScript != currentlyEquippedHat)
                {
                    hatScript.ForceReturnToLineup();
                }
            }
        }
    }

    public void Deactivate()
    {
        isCustomisationMenuOpen = false;

        if (currentlyEquippedHat != null)
        {
            Collider[] colliders = currentlyEquippedHat.GetComponentsInChildren<Collider>();
            foreach(Collider col in colliders)
            {
                col.enabled = false;
            }
            currentlyEquippedHat.ApplyParenting();

        }

        foreach (GameObject hatObj in allHats)
        {
            if (hatObj != null)
            {
                HatInteractive hatScript = hatObj.GetComponentInChildren<HatInteractive>();
                
                if (hatScript != null && hatScript == currentlyEquippedHat)
                {
                    hatObj.SetActive(true);
                    
                    Collider col = hatObj.GetComponentInChildren<Collider>();
                    if (col != null) col.enabled = false;
                }
                else
                {
                    hatObj.SetActive(false);
                }
            }
        }
    }
}