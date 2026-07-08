using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class frisbeeMarker : MonoBehaviour
{
    // A "static" variable is shared across ALL instances of this script.
    // Since there's only ever one frisbee in the scene, this variable
    // acts as a single global answer to "where is the frisbee right now?"
    public static Transform Current;

    // OnEnable is a built-in Unity function that runs automatically the
    // moment this object becomes active in the scene (e.g. right after
    // Instantiate() creates it). We use it to say "I exist, here's my transform."
    void OnEnable()
    {
        Current = transform;
    }

    // OnDisable runs automatically when this object is destroyed or
    // deactivated. We use it to clear the reference so nothing keeps
    // pointing at a frisbee that no longer exists.
    void OnDisable()
    {
        if (Current == transform)
            Current = null;
    }
}