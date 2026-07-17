using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    [Header("Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject DonutBoxPrefab;
    public float cooldownTime = 2.0f;
    private GameObject currentSpawnedDonutBox;
    private float nextAllowedClickTime = 0f;

    private bool isTraining = false;

    public void StartTraining()
    {
        if (Time.time < nextAllowedClickTime)//Fixes the double click issue
        {
            Debug.Log("Button is on cooldown, Ignoring click.");
            return; 
        }
        nextAllowedClickTime = Time.time + cooldownTime;

        if (isTraining)
        {
            Destroy(currentSpawnedDonutBox);
            isTraining = false;
            if (showDebugLogs)
            {
                Debug.Log("Training stopped");
            }
            return;

        }
        else
        {
            isTraining = true;
        }

        if (showDebugLogs)
        {
            Debug.Log("Training started");
        }

        if (currentSpawnedDonutBox != null)
        {
            Destroy(currentSpawnedDonutBox);
        }

        if (spawnPoint != null)
        {
            currentSpawnedDonutBox = Instantiate(DonutBoxPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            currentSpawnedDonutBox = Instantiate(DonutBoxPrefab);
        }
        
        if (showDebugLogs)
        {
            Debug.Log("Prefab spawned!");
        }

    }

    




    public bool IsTraining()
    {
        return isTraining;
    }

}
