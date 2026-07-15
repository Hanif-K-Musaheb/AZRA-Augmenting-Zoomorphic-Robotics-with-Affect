using UnityEngine;

public class ObjectToggler : MonoBehaviour
{
    [Header("Settings")]
    public GameObject prefabToInstantiate; 
    public Transform spawnPoint;           
    
    [Header("Cooldown Settings")]
    [Tooltip("How many seconds you must wait before the button can be clicked again.")]
    public float cooldownTime = 2.0f; // Set to 2 seconds

    // Tracks the specific object we spawned
    private GameObject currentSpawnedObject;
    
    // Tracks the exact moment the button was last clicked successfully
    private float nextAllowedClickTime = 0f;

    public void ToggleObject()
    {
        // 1. Check if enough time has passed since the last click
        // Time.time gives us the total seconds since the game started running
        if (Time.time < nextAllowedClickTime)
        {
            Debug.Log("Button is on cooldown! Ignoring click.");
            return; // This immediately stops the code and prevents the double-click
        }

        // 2. We are allowed to click! Set the timer for the next allowed click
        nextAllowedClickTime = Time.time + cooldownTime;

        // 3. Run our normal spawn/destroy logic
        if (currentSpawnedObject == null)
        {
            // Spawn it
            if (spawnPoint != null)
            {
                currentSpawnedObject = Instantiate(prefabToInstantiate, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                currentSpawnedObject = Instantiate(prefabToInstantiate);
            }
            
            Debug.Log("Prefab spawned!");
        }
        else
        {
            // Destroy it
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null; 
            Debug.Log("Prefab destroyed!");
        }
    }
}