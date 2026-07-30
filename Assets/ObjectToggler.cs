using UnityEngine;
using System.Collections;

public class ObjectToggler : MonoBehaviour
{
    [Header("Settings")]
    public GameObject prefabToInstantiate; 
    public Transform spawnPoint;          
    
    [Header("Cooldown Settings")]
    private GameObject currentTV;
    public float timeToWait = 300f; // 5 minutes, the amount of the video wanted in the study

    // We need to store a reference to the timer so we can cancel it!
    private Coroutine currentTimer; 

    public void ToggleObject()
    {
        // 1. Force a cleanup first just in case there is already a TV or a running timer
        Deactivate();

        // 2. Spawn the new TV
        if (spawnPoint != null)
        {
            currentTV = Instantiate(prefabToInstantiate, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            currentTV = Instantiate(prefabToInstantiate);
        }
        
        // 3. Start the timer and save it to our currentTimer variable
        currentTimer = StartCoroutine(DeactivateTimer());
    }

    IEnumerator DeactivateTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        Deactivate();
    }

    public void Deactivate()
    {
        // If a timer is currently running, STOP IT so it doesn't destroy future TVs
        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
            currentTimer = null;
        }

        // Destroy the TV
        if (currentTV != null)
        {
            Destroy(currentTV);
            currentTV = null; 
        }
    }
}