using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrisbeeFlight : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasBeenGrabbed = false;

    [Header("Flight Settings")]
    public float liftMultiplier = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        // Ensure rotation is locked at start so it stays horizontal when spawned
        LockHorizontalRotation();
    }

    void FixedUpdate()
    {
        if (hasBeenGrabbed && !rb.isKinematic)
        {
            float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
            if (horizontalSpeed > 1.0f)
            {
                rb.AddForce(Vector3.up * (horizontalSpeed * liftMultiplier), ForceMode.Acceleration);
            }
        }
    }

    public void OnGrab()
    {
        hasBeenGrabbed = true;
        rb.useGravity = true;
        
        // Unlock rotation so you can rotate and inspect it
        rb.constraints = RigidbodyConstraints.None;
    }

    public void OnRelease()
    {
        // Lock rotation again so it stays horizontal in flight
        LockHorizontalRotation();
    }

    private void LockHorizontalRotation()
    {
        // Freezes X and Z rotation, leaves Y free to spin
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}