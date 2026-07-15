using UnityEngine;
using UnityEngine.Video;

public class TVController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject virtualTV; // Drag your VirtualTV Quad here
    [SerializeField] private VideoPlayer videoPlayer; // Drag your VirtualTV Quad here as well

    private bool isTVActive = false;

    void Start()
    {
        // Ensure the TV is hidden when the game starts
        if (virtualTV != null)
        {
            virtualTV.SetActive(false);
        }
    }

    // Connect this method to your UI Menu Button's "On Click()" event
    public void ToggleTV()
    {
        isTVActive = !isTVActive;
        virtualTV.SetActive(isTVActive);

        if (isTVActive)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause(); // or .Stop() if you want it to restart from the beginning next time
        }
    }
}