using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    [Header("Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform qooboTransform;
    [SerializeField] private GameObject DonutBoxPrefab;
    private GameObject currentSpawnedDonutBox;


    private bool is_Training = false;

    public void StartTraining()
    {
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

        InstantiateDonutBox();
       
    }

    private void InstantiateDonutBox()
    {
        if (currentSpawnedDonutBox != null)
        {
            Destroy(currentSpawnedDonutBox);
        }

        if (qooboTransform == null)return;
        
        float rightOffset = -0.6f;   
        float forwardOffset = 0.1f; 

        //calculate the position of the donut box
        Vector3 calculatedSpawnPosition = qooboTransform.position 
                                        + (qooboTransform.forward * forwardOffset)
                                        + (qooboTransform.right * rightOffset);

        calculatedSpawnPosition.y = qooboTransform.position.y;//force it to be on the table with Qoobo          
        Quaternion rotationOffset = Quaternion.Euler(0, -135f, 0); 
        Quaternion finalRotation = qooboTransform.rotation * rotationOffset; 

        currentSpawnedDonutBox = Instantiate(DonutBoxPrefab, calculatedSpawnPosition, finalRotation);

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
