using UnityEngine;

public class FeedingManager : MonoBehaviour
{
    public static FeedingManager Instance { get; private set; }

    [Header("Shared References")]
    [SerializeField] private GameObject robotBodyGameObject;
    [SerializeField] private AudioController audioController;
    [SerializeField] private EmotionModel emotionModel;
    [SerializeField] private EmotionController emotionController;

    public GameObject RobotBody => robotBodyGameObject;
    public AudioController Audio => audioController;
    public EmotionModel EmotionModel => emotionModel;
    public EmotionController EmotionController => emotionController;

    // Ensures only one donut is being actively eaten at any moment,
    // even if several are within proximity range simultaneously
    public bool IsSomeoneBeingEaten { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NotifyEatingStarted() => IsSomeoneBeingEaten = true;
    public void NotifyEatingEnded() => IsSomeoneBeingEaten = false;
}