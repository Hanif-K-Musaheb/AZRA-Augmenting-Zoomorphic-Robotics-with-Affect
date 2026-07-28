using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HatInteractive : MonoBehaviour
{
    [SerializeField] private HatManager hatManager;
    
    [Header("Hat Settings")]
    [SerializeField] private Transform QooboCustomisation;    [SerializeField] private Transform HatTargetPosition;
    [SerializeField] private float proximityThreshold = 0.15f;
    [SerializeField] private string equipSoundName = "peep"; 

    [Header("Spawn Anchor")]
    [SerializeField] private Transform originalSpawnPoint;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private bool isEquipped = false;

    void Update()
    {
        if (HatTargetPosition == null) return;

        float distance = Vector3.Distance(transform.position, HatTargetPosition.position);

        if (!isEquipped)
        {
            if (distance <= proximityThreshold && hatManager.currentlyEquippedHat == null)
            {
                if (showDebugLogs) Debug.Log($"HatInteractive ({name}): Snapping to head.");
                PlaceHatOnHead();
            }
        }
        else
        {
            if (hatManager.isCustomisationMenuOpen && distance > proximityThreshold + 0.05f)
            {
                if (showDebugLogs) Debug.Log($"HatInteractive ({name}): Pulled off head.");
                RemoveHatFromHead();
            }
        }
    }

    private void PlaceHatOnHead()
    {
        isEquipped = true;

        hatManager.RegisterEquippedHat(this);
        PlayEquipSound();
    }

    private void RemoveHatFromHead()
    {
        isEquipped = false;
        hatManager.ClearEquippedHat(this);
    }

    public void ForceReturnToLineup()//not called yet
    {
        isEquipped = false;
        if (originalSpawnPoint != null)
        {
            transform.position = originalSpawnPoint.position;
            transform.rotation = originalSpawnPoint.rotation;
        }
    }

    public void ApplyParenting()
    {
        if (isEquipped && HatTargetPosition != null)
        {
            transform.parent.SetParent(HatTargetPosition);
            if (showDebugLogs) Debug.Log($"HatInteractive ({name}): Parented to head for gameplay.");
        }
    }

    
    public void RemoveParenting()
    {
        if (isEquipped)
        {
            transform.parent.SetParent(QooboCustomisation);
            if (showDebugLogs) Debug.Log($"HatInteractive ({name}): Unparented for customisation.");
        }
    }

    private void PlayEquipSound()
    {
        if (FeedingManager.Instance != null && FeedingManager.Instance.Audio != null)
        {
            FeedingManager.Instance.Audio.PlaySound(equipSoundName);
        }
    }
}