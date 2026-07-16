using UnityEngine;
using System.Collections;

public class DonutInstance : MonoBehaviour
{
    [Header("Donut Models (children of this prefab)")]
    [SerializeField] private GameObject fullDonutModel;
    [SerializeField] private GameObject partialDonutModel;
    [SerializeField] private GameObject mostlyEatenDonutModel;

    [Header("Feeding Settings")]
    [SerializeField] private float proximityThreshold = 0.15f;
    [SerializeField] private float biteInterval = 1.5f;
    [SerializeField] private string biteSoundName = "peep";
    [SerializeField] private float timeBetweenBites = 2.0f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private enum DonutState { Full, Partial, MostlyEaten, Gone }
    private DonutState currentState = DonutState.Full;
    private bool isEatingSequence = false;

    void Start()
    {
        SetModelState(DonutState.Full);
    }

    void Update()
    {
        // Needs the shared manager to know where the robot is
        if (FeedingManager.Instance == null || FeedingManager.Instance.RobotBody == null)
            return;

        if (isEatingSequence) return;

        // Only one donut can be actively eaten at a time - others wait their turn
        if (FeedingManager.Instance.IsSomeoneBeingEaten) return;

        float distance = Vector3.Distance(transform.position, FeedingManager.Instance.RobotBody.transform.position);

        if (showDebugLogs)
            Debug.Log($"DonutInstance ({name}): Distance to robot: {distance:F3}m (threshold: {proximityThreshold:F3}m)");

        if (distance <= proximityThreshold)
        {
            StartCoroutine(EatingSequence());
        }
    }

    private IEnumerator EatingSequence()
    {
        isEatingSequence = true;
        FeedingManager.Instance.NotifyEatingStarted();

        if (showDebugLogs) Debug.Log($"DonutInstance ({name}): Started eating sequence");

        yield return new WaitForSeconds(biteInterval);
        PlayBiteSound();
        SetModelState(DonutState.Partial);

        yield return new WaitForSeconds(biteInterval);
        PlayBiteSound();
        SetModelState(DonutState.MostlyEaten);

        yield return new WaitForSeconds(biteInterval);
        PlayBiteSound();
        SetModelState(DonutState.Gone);

        yield return new WaitForSeconds(timeBetweenBites);

        var response = FeedingManager.Instance.EmotionModel.CalculateEmotionalResponse("Feeding");
        FeedingManager.Instance.EmotionController.TryDisplayEmotion(response.EmotionToDisplay, response.TriggerEvent);

        FeedingManager.Instance.NotifyEatingEnded();

        Destroy(gameObject);
    }

    private void SetModelState(DonutState newState)
    {
        if (fullDonutModel != null) fullDonutModel.SetActive(false);
        if (partialDonutModel != null) partialDonutModel.SetActive(false);
        if (mostlyEatenDonutModel != null) mostlyEatenDonutModel.SetActive(false);

        switch (newState)
        {
            case DonutState.Full: if (fullDonutModel != null) fullDonutModel.SetActive(true); break;
            case DonutState.Partial: if (partialDonutModel != null) partialDonutModel.SetActive(true); break;
            case DonutState.MostlyEaten: if (mostlyEatenDonutModel != null) mostlyEatenDonutModel.SetActive(true); break;
            case DonutState.Gone: break; 
        }

        currentState = newState;
        if (showDebugLogs) Debug.Log($"DonutInstance ({name}): Model state -> {newState}");
    }

    private void PlayBiteSound()
    {
        if (FeedingManager.Instance.Audio != null)
            FeedingManager.Instance.Audio.PlaySound(biteSoundName);
    }
}