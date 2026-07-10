using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomisationMenuController : MonoBehaviour
{    
    [Header("Emotional State Buttons")]
    [SerializeField] private Button CowboyHatButton;
    [SerializeField] private Button MagicianHatButton;
    [SerializeField] private Button SombreroHatButton;
    [SerializeField] private Button NoHatButton;

    [Header("Customisation Assets")]
    [SerializeField] private GameObject CowboyHat;
    [SerializeField] private GameObject MagicianHat;
    [SerializeField] private GameObject SombreroHat;

    
    
    void Start()
    {
        // Setup emotional state buttons
        SetupEmotionalStateButtons();
        
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
        switch (hatType)
        {
            case "Cowboy":
                MagicianHat.SetActive(false);
                SombreroHat.SetActive(false);
                CowboyHat.SetActive(true); // Turns the cowboy hat on
                Debug.Log("CustomisationMenu: Equipped Cowboy Hat!");
                break;
            case "Magician":
                CowboyHat.SetActive(false);
                SombreroHat.SetActive(false);
                MagicianHat.SetActive(true); // Turns the magician hat on
                Debug.Log("CustomisationMenu: Equipped Magician Hat!");
                break;
            case "Sombrero":
                CowboyHat.SetActive(false); 
                MagicianHat.SetActive(false); 
                SombreroHat.SetActive(true); // Turns the sombrero hat on
                Debug.Log("CustomisationMenu: Equipped Sombrero Hat!");
                break;

            case "None":
                CowboyHat.SetActive(false); // Turns the cowboy hat off
                MagicianHat.SetActive(false); // Turns the magician hat off
                SombreroHat.SetActive(false); // Turns the sombrero hat off
                Debug.Log("CustomisationMenu: Removed all hats.");
                break;
        }
    }
    

}
   

    
  

    
 
    
