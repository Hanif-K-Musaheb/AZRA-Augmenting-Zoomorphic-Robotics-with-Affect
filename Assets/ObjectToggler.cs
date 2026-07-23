using UnityEngine;

public class ObjectToggler : MonoBehaviour
{
    [Header("Settings")]
    public GameObject prefabToInstantiate; 
    public Transform spawnPoint;           
    
    [Header("Cooldown Settings")]
    public float cooldownTime = 2.0f; 
    // Tracks the specific object we spawned
    private GameObject currentSpawnedObject;
    
    // Tracks the exact moment the button was last clicked successfully
    private float nextAllowedClickTime = 0f;

    public void ToggleObject()
    {
     
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

    public void Deactivate()
    {
        if(currentSpawnedObject != null)
        {
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null; 
        }
    }
}