using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System;

public class EndOfFeatureQuesitonaire : MonoBehaviour
{
    [Header("Study Setup")]
    public string participantID = "P1";
    public string currentFeature = "Feature 1"; // Update this from your GameManager when a feature ends
    private string csvFilePath;

    [Header("UI Elements")]
    public GameObject questionnairePanel; // The main UI parent to toggle on/off
    public TextMeshProUGUI questionText;

    [Header("Likert Buttons Q1")]
    public Button btnStronglyDisagree; // 1
    public Button btnDisagree;         // 2
    public Button btnNeutral;          // 3
    public Button btnAgree;            // 4
    public Button btnStronglyAgree;    // 5

    [Header("Questions")]
    [Tooltip("Add your 3 questions here in the inspector.")]
    public List<string> questions = new List<string> {
        "Did Quoobo’s behavior seem logical to you based on your own interaction with it?",
        "Was the perceived personality of the Quoobo what you would prefer?",
        "Did Quoobo present high signs of emotional intelligence?"
    };

    private float clickCooldown = 1.5f;
    private float nextAllowedClickTime = 0f;

    private int currentQuestionIndex = 0;
    private List<int> currentAnswers = new List<int>();

    void Start()
    {
        // CRITICAL FOR QUEST 3: Application.persistentDataPath is the only guaranteed 
        // folder Android allows Unity to write files to without special permissions.
        csvFilePath = Path.Combine(Application.persistentDataPath, "Qoobo_StudyData_Hanif.csv");

        // Initialize the CSV with headers if it doesn't exist yet
        if (!File.Exists(csvFilePath))
        {
            File.WriteAllText(csvFilePath, "ParticipantID,Timestamp,Feature,Q1,Q2,Q3\n");
        }

        SetupButtons();
        
        // Hide the UI at start
        questionnairePanel.SetActive(false);
    }

    void SetupButtons()
    {
        // Clear old listeners just in case, then assign the score values (1 through 5)
        btnStronglyDisagree.onClick.RemoveAllListeners();
        btnStronglyDisagree.onClick.AddListener(() => RecordAnswer(1));

        btnDisagree.onClick.RemoveAllListeners();
        btnDisagree.onClick.AddListener(() => RecordAnswer(2));

        btnNeutral.onClick.RemoveAllListeners();
        btnNeutral.onClick.AddListener(() => RecordAnswer(3));

        btnAgree.onClick.RemoveAllListeners();
        btnAgree.onClick.AddListener(() => RecordAnswer(4));

        btnStronglyAgree.onClick.RemoveAllListeners();
        btnStronglyAgree.onClick.AddListener(() => RecordAnswer(5));
    }

    // Call this method from your main GameManager when a feature finishes
    public void StartQuestionnaire(string featureName)
    {
        currentFeature = featureName;
        currentQuestionIndex = 0;
        currentAnswers.Clear();
        nextAllowedClickTime = 0f;
        
        questionnairePanel.SetActive(true);
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            questionText.text = questions[currentQuestionIndex];
        }
        else
        {
            FinishQuestionnaire();
        }
    }

    void RecordAnswer(int score)
    {
        if (Time.time < nextAllowedClickTime) 
        {
            Debug.Log("Click ignored: Cooldown active.");
            return; 
        }
        nextAllowedClickTime = Time.time + clickCooldown;

        currentAnswers.Add(score);
        currentQuestionIndex++;
        DisplayQuestion();
    }

    void FinishQuestionnaire()
    {
        questionText.text = "Thank you! Saving responses...";
        SaveDataToCSV();
        
        // Hide the panel after 2 seconds so the user can read the thank you message
        Invoke("HidePanel", 2f);
    }

    void HidePanel()
    {
        questionnairePanel.SetActive(false);
    }

    void SaveDataToCSV()
    {
        // Get the current date and time
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Build the base of the string: P1, 2026-07-24 10:50:45, Feature 1
        string dataRow = $"{participantID},{timestamp},{currentFeature}";

        // Append each answer dynamically (works if you change it from 3 to 5 questions later!)
        foreach (int answer in currentAnswers)
        {
            dataRow += $",{answer}";
        }
        
        // Add a line break at the end of the row
        dataRow += "\n";

        // AppendAllText automatically creates the file if missing and adds to the bottom without overwriting
        File.AppendAllText(csvFilePath, dataRow);
        Debug.Log($"Data successfully saved to: {csvFilePath}");
    }
}