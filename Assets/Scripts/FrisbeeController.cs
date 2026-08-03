using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class FrisbeeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] [Tooltip("Drag the Frisbee prefab here")]
    private GameObject frisbeePrefab;
    
    [SerializeField] [Tooltip("Drag the GameObject where you want the frisbee to spawn here")]
    private Transform spawnPoint; // <-- NEW: Dedicated spawn point

    [SerializeField] [Tooltip("Drag the Main Camera here")] 
    private Transform playerCamera;

    [Header("Spawning Settings")]
    private float spawnDistance = 2.0f; 

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private float lastSpawnTime = 0f;
    [SerializeField] private float spawnCooldown = 0.5f;

    private GameObject currentFrisbee;
    public event System.Action<Transform> OnFrisbeeSpawned;

    public static Transform Current;

    void OnEnable()  => Current = transform;
    void OnDisable() { if (Current == transform) Current = null; }

    void Start()
    {
        if (spawnPoint == null && showDebugLogs)
        {
            Debug.LogWarning("FrisbeeController: Spawn Point not assigned! Will rely on camera fallback.");
        }

        // Failsafe: automatically find the main camera if you forget to drag it into the inspector
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
        if (frisbeePrefab == null)
        {
            Debug.LogError("FrisbeeController: Frisbee prefab not assigned! Please assign it in the inspector.");
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        bool positionFound = false;

        // Try Method A: Spawn at the dedicated spawn point
        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
            positionFound = true;
            if (showDebugLogs) Debug.Log($"FrisbeeController: Using spawn point position: {spawnPosition}");
        }

        // Try Method B: Fallback to camera center view
        if (!positionFound && playerCamera != null)
        {
            spawnPosition = playerCamera.position + (playerCamera.forward * spawnDistance);
            positionFound = true;
            if (showDebugLogs) Debug.Log($"FrisbeeController: Using camera fallback position: {spawnPosition}");
        }
        
        // Safety check if absolutely nothing is tracked/assigned
        if (!positionFound)
        {
            Debug.LogWarning("FrisbeeController: No spawn point or camera assigned. Cannot determine spawn position.");
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

    public void OnSpawnButtonClicked()
    {
        if (showDebugLogs) Debug.Log("FrisbeeController: Spawn UI button clicked");
        SpawnFrisbee();
    }

    public void Deactivate() 
    {
        if (currentFrisbee != null)
        {
            Destroy(currentFrisbee);
            currentFrisbee = null; 
        }

        if (showDebugLogs) 
        {
            Debug.Log("FrisbeeController: frisbee destroyed");
        }
    }
}