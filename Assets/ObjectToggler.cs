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



    public void ToggleObject()
    {
     
        if (currentTV == null)
        {
            // Spawn it
            if (spawnPoint != null)
            {
                currentTV = Instantiate(prefabToInstantiate, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                currentTV = Instantiate(prefabToInstantiate);
            }
            StartCoroutine(DeactivateTimer());
            
        }
        else
        {
            // Destroy it
            Destroy(currentTV);
            currentTV = null; 
        }
    }
    IEnumerator DeactivateTimer()//stops the TV after 5min
    {
        yield return new WaitForSeconds(timeToWait);
        Deactivate();
    }

    public void Deactivate()
    {
        if(currentTV != null)
        {
            Destroy(currentTV);
            currentTV = null; 
        }
    }
}