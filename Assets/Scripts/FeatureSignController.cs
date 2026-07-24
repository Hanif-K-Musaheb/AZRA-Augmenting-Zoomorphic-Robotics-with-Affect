using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeatureSignController : MonoBehaviour
{
    [Header("Tour Panel References (Reused as Sign)")]

    [SerializeField] private GameObject tourPanel;
    [SerializeField] private Image panelBackground;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    void Start()
    {
        // Ensure the sign is hidden when the app first boots up
        HideSign();
    }

    /// <summary>
    /// Displays the sign with custom text. 
    /// Note: Spelled exactly to match the call in your MenuController script.
    /// </summary>
    public void ShowExplaination(string featureTitle, string featureDescription, Color newColor)
    {
        // 1. Safely update the text if the references exist
        if (titleText != null) titleText.text = featureTitle;
        if (descriptionText != null) descriptionText.text = featureDescription;
        if (panelBackground != null) panelBackground.color = newColor;
        
        // 2. Turn on the physical panel
        if (tourPanel != null) tourPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the sign when a feature is turned off.
    /// </summary>
    public void HideSign()
    {
        if (tourPanel != null) tourPanel.SetActive(false);
    }
}