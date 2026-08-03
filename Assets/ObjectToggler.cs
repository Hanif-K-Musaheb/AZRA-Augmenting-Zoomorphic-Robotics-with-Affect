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
    private Coroutine currentTimer; 

    public void ToggleObject()
    {
        Deactivate();

        if (spawnPoint != null)
        {
            currentTV = Instantiate(prefabToInstantiate, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            currentTV = Instantiate(prefabToInstantiate);
        }
        
        currentTimer = StartCoroutine(DeactivateTimer());
    }

    IEnumerator DeactivateTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        Deactivate();
    }

    public void Deactivate()
    {
        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);//stop timer to deactivate TV so it doesnt delete future instances
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