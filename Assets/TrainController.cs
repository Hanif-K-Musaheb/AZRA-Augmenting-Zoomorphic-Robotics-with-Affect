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

    private bool is_Training = false;

    public void StartTraining()
    {
        // if (Time.time < nextAllowedClickTime)//stops issue with AR double click
        // {
        //     Debug.Log("Button is on cooldown, Ignoring click.");
        //     return; 
        // }
        // nextAllowedClickTime = Time.time + cooldownTime;

        if (is_Training)
        {
            Deactivate();
            return;
        }
        else
        {
            is_Training = true;
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

    public void Deactivate()
    {
        if (!is_Training) return;

        Destroy(currentSpawnedDonutBox);
        is_Training = false;

        if (showDebugLogs)
        {
            Debug.Log("Training stopped");
        }
    }

    public bool IsTraining()
    {
        return is_Training;
    }

}
