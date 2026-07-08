using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class FrisbeeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] [Tooltip("Drag your saved Frisbee prefab here")] 
    private GameObject frisbeePrefab;
    
    [SerializeField] [Tooltip("Drag the Main Camera here")] 
    private Transform playerCamera;

    [Header("Spawning Settings")]
    [SerializeField] [Tooltip("How many meters in front of the camera it should spawn as a fallback")] 
    private float spawnDistance = 0.5f; 
    
    [SerializeField] [Tooltip("How high above the palm to spawn when using hand tracking")] 
    private float handHeightOffset = 0.1f; 

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private XRHandSubsystem handSubsystem;
    private float lastSpawnTime = 0f;
    [SerializeField] private float spawnCooldown = 0.5f;

    private GameObject currentFrisbee;
    public event System.Action<Transform> OnFrisbeeSpawned;

    public static Transform Current;

    void OnEnable()  => Current = transform;
    void OnDisable() { if (Current == transform) Current = null; }

    void Start()
    {
        // Get hand tracking subsystem to find hand positions
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            if (showDebugLogs) Debug.Log("FrisbeeController: Hand tracking subsystem initialized successfully.");
        }
        else
        {
            if (showDebugLogs) Debug.LogWarning("FrisbeeController: No hand tracking subsystem found. Will use camera fallback.");
        }

        // Failsafe: automatically find the main camera if you forget to drag it into the inspector (could delete)
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        // Check for spawn key press using new Input System ('F' for Frisbee)
        if (Keyboard.current != null && Keyboard.current[Key.F].wasPressedThisFrame)
        {
            if (showDebugLogs) Debug.Log("F key pressed - spawning frisbee");
            SpawnFrisbee();
        }
    }

    public void SpawnFrisbee()
    {
        if (Time.time - lastSpawnTime < spawnCooldown)
        {
            if (showDebugLogs) Debug.Log("FrisbeeController: Spawn cooldown active. Please wait before spawning another frisbee.");
            return;
        }
        lastSpawnTime = Time.time;

        if (frisbeePrefab == null)
        {
            Debug.LogError("FrisbeeController: Frisbee prefab not assigned! Please assign it in the inspector.");
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        bool positionFound = false;

        // Try Method A: Spawn near right hand
        if (handSubsystem != null && handSubsystem.rightHand.isTracked)
        {
            XRHandJoint rightPalm = handSubsystem.rightHand.GetJoint(XRHandJointID.Palm);
            if (rightPalm.TryGetPose(out Pose palmPose))
            {
                spawnPosition = palmPose.position + (Vector3.up * handHeightOffset);
                positionFound = true;
                if (showDebugLogs) Debug.Log($"FrisbeeController: Using right hand position: {spawnPosition}");
            }
        }

        // Try Method B: Fallback to camera center view
        if (!positionFound && playerCamera != null)
        {
            spawnPosition = playerCamera.position + (playerCamera.forward * spawnDistance);
            positionFound = true;
            if (showDebugLogs) Debug.Log($"FrisbeeController: Using camera fallback position: {spawnPosition}");
        }
        
        // Safety check if absolutely nothing is tracked
        if (!positionFound)
        {
            Debug.LogWarning("FrisbeeController: No camera or hand tracked. Cannot determine spawn position.");
            return;
        }
        // Get rid of the current frisbee so Qoobo only tracks 1
        if (currentFrisbee != null)
        {
            Destroy(currentFrisbee);
        }

        // Spawn the frisbee at the calculated position
        currentFrisbee = Instantiate(frisbeePrefab, spawnPosition, Quaternion.identity);
        OnFrisbeeSpawned?.Invoke(currentFrisbee.transform);

        
        if (showDebugLogs) Debug.Log("FrisbeeController: Frisbee spawned successfully!");
    }

    // Public method explicitly named for UI button events
    public void OnSpawnButtonClicked()
    {
        if (showDebugLogs) Debug.Log("FrisbeeController: Spawn UI button clicked");
        SpawnFrisbee();
    }
}