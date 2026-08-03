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
    public string currentFeature = "Feature 1";
    private string csvFilePath;

    [Header("UI Elements")]
    public GameObject questionnairePanel; 
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
        "Qoobo’s behavior seem logical to you based on your own interaction with it?",
        "The perceived personality of the Qoobo was what you would prefer?",
        "Qoobo presented high signs of emotional intelligence?"
    };

    private float clickCooldown = 1.5f;
    private float nextAllowedClickTime = 0f;

    private int currentQuestionIndex = 0;
    private List<int> currentAnswers = new List<int>();

    void Start()
    {
        csvFilePath = Path.Combine(Application.persistentDataPath, "Qoobo_StudyData_Hanif.csv");

        if (!File.Exists(csvFilePath))//initialize the CSV header if it doesn't exist yet
        {
            File.WriteAllText(csvFilePath, "ParticipantID,Timestamp,Feature,Q1,Q2,Q3\n");
        }

        SetupButtons();
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
        
        Invoke("HidePanel", 2f);//hide the panel after 2 seconds so the user can read the thank you message
    }

    void HidePanel()
    {
        questionnairePanel.SetActive(false);
    }

    void SaveDataToCSV()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Build the base of the string: P1, 2026-07-24 10:50:45, Feature 1
        string dataRow = $"{participantID},{timestamp},{currentFeature}";

        foreach (int answer in currentAnswers)
        {
            dataRow += $",{answer}";
        }
        
        dataRow += "\n";

        // AppendAllText automatically creates the file if missing and adds to the bottom without overwriting
        File.AppendAllText(csvFilePath, dataRow);
        Debug.Log($"Data successfully saved to: {csvFilePath}");
    }
}