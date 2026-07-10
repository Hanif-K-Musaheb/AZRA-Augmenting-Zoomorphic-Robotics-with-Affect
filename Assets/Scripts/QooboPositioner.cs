using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class QooboPositioner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] [Tooltip("Top-level visual/prefab root for the AR Qoobo.")] private GameObject qooboMesh;
    [SerializeField] [Tooltip("Reference to SceneController to trigger wake-up sequence on placement.")] private SceneController sceneController;
    
    [Header("Settings")]
    [SerializeField] private float handHeightOffset = -0.3f; 
    [SerializeField] private float handForwardOffset = 0.2f; 
    [SerializeField] private float rotationOffset = 60f;
    [SerializeField] private float pinchThreshold = 0.02f; 
    [SerializeField] private bool enablePinchPlacement = true; 
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private bool isPositioned = false;
    private bool isRepositioning = false;
    private bool hasPinchPositioned = false; 
    private XRHandSubsystem handSubsystem;

    void Start()
    {
        if (qooboMesh == null)
        {
            Debug.LogError("QooboMesh reference not set in QooboPositioner!");
            enabled = false;
            return;
        }

        if (sceneController == null)
        {
            Debug.LogError("SceneController reference not set in QooboPositioner!");
            enabled = false;
            return;
        }

        // Try to get the hand tracking subsystem
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            if (showDebugLogs) Debug.Log("Hand tracking subsystem initialized successfully");
        }
        else
        {
            // WE DO NOT DISABLE THE SCRIPT HERE ANYMORE. 
            // This allows our fallback input system methods to still keep running!
            Debug.LogWarning("Subsystem not found. Relying on Unity Input Device fallbacks.");
        }
    }

    void Update()
    {
        // 1. BACKUP METHOD: Check for Space key using the standard Input System
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space key pressed - Forcing position update via fallback system.");
            UpdateQooboPosition();
            return;
        }

        // 2. ORIGINAL METHOD: Only run pinch tracking if the subsystem is working
        if (handSubsystem != null && enablePinchPlacement)
        {
            bool leftHandTracked = handSubsystem.leftHand.isTracked;
            bool rightHandTracked = handSubsystem.rightHand.isTracked;

            if (leftHandTracked && rightHandTracked)
            {
                XRHandJoint leftThumbTip = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);
                XRHandJoint leftIndexTip = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
                
                Vector3 leftThumbPos = leftThumbTip.TryGetPose(out Pose thumbPose) ? thumbPose.position : Vector3.zero;
                Vector3 leftIndexPos = leftIndexTip.TryGetPose(out Pose indexPose) ? indexPose.position : Vector3.zero;
                
                float pinchDistance = Vector3.Distance(leftThumbPos, leftIndexPos);
                bool isPinching = pinchDistance < pinchThreshold;

                if (isPinching && !hasPinchPositioned) 
                {
                    if (!sceneController.IsWakeUpComplete())
                    {
                        Debug.Log("Pinch detected via subsystem - Updating position.");
                        UpdateQooboPosition();
                        hasPinchPositioned = true; 
                    }
                }
            }
        }
    }

    public void UpdateQooboPosition()
    {
        Vector3 rightPalmPosition = Vector3.zero;
        Quaternion rightPalmRotation = Quaternion.identity;
        bool foundPosition = false;

        // Try Method A: Subsystem Bone Tracking
        if (handSubsystem != null && handSubsystem.rightHand.isTracked)
        {
            XRHandJoint rightPalm = handSubsystem.rightHand.GetJoint(XRHandJointID.Palm);
            if (rightPalm.TryGetPose(out Pose palmPose) && palmPose.position != Vector3.zero)
            {
                rightPalmPosition = palmPose.position;
                rightPalmRotation = palmPose.rotation;
                foundPosition = true;
                Debug.Log("Hand position fetched via Subsystem.");
            }
        }

        // Try Method B: Input Device Characteristic Fallback (The system driving your working pointer)
        if (!foundPosition)
        {
            var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Right, rightHandDevices);
            if (rightHandDevices.Count > 0)
            {
                if (rightHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 pos))
                {
                    rightPalmPosition = pos;
                    rightHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion rot);
                    rightPalmRotation = rot;
                    foundPosition = true;
                    Debug.Log("Hand position fetched via Input Device characteristic.");
                }
            }
        }

        // Try Method C: Absolute emergency fallback (Spawns right in front of your headset view)
        if (!foundPosition)
        {
            Debug.LogWarning("No hand tracking device stream found! Using headset center fallback view.");
            rightPalmPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            rightPalmRotation = Camera.main.transform.rotation;
        }

        // Calculate positions using horizontal/vertical offsets
        Vector3 downDirection = rightPalmRotation * Vector3.down;
        Vector3 forwardDirection = rightPalmRotation * Vector3.forward;
        
        Vector3 targetPos = rightPalmPosition + (downDirection * Mathf.Abs(handHeightOffset)) + 
                            (forwardDirection * handForwardOffset);
        
        // Move the mesh
        qooboMesh.transform.position = targetPos;

        // Rotate the mesh matching your alignment settings
        Vector3 palmForward = rightPalmRotation * Vector3.forward;
        palmForward.y = 0; 
        if (palmForward != Vector3.zero)
        {
            Quaternion yawRotation = Quaternion.LookRotation(palmForward, Vector3.up);
            Vector3 currentEuler = qooboMesh.transform.eulerAngles;
          
            float yaw = yawRotation.eulerAngles.y+ rotationOffset;
            qooboMesh.transform.rotation = Quaternion.Euler(currentEuler.x, yaw, currentEuler.z);
        }

        // Fire the projection sequence!
        sceneController.StartWakeUpSequence();
        Debug.Log($"Qoobo successfully positioned at: {targetPos}");
    }

    public bool IsPositioned()
    {
        return isPositioned && !isRepositioning;
    }
}