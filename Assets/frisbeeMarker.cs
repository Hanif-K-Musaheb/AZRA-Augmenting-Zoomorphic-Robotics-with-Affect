using UnityEngine;
using Oculus.Interaction; // ISDK namespace - contains Grabbable, PointerEvent, etc.

public class frisbeeMarker : MonoBehaviour
{
    public static Transform Current;
    public static bool IsReleased;

    private Grabbable grabbable;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
    }

    void OnEnable()
    {
        Current = transform;
        IsReleased = false; // starts un-thrown when spawned/re-enabled

        if (grabbable != null)
        {
            // WhenPointerEventRaised fires for every pointer interaction
            // (select, unselect, hover, etc). We filter for the release type below.
            grabbable.WhenPointerEventRaised += HandlePointerEvent;
        }
    }

    void OnDisable()
    {
        if (Current == transform) Current = null;

        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        // Unselect = hand let go of the object (covers both dropping and throwing)
        if (evt.Type == PointerEventType.Unselect)
        {
            IsReleased = true;
        }
    }
}