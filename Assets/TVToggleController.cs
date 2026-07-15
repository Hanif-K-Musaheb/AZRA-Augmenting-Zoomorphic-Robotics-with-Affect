using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TVToggleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tvPlane; // the plane showing the video
    [SerializeField] private Button toggleButton; // the button on your main menu
    [SerializeField] private VideoPlayer videoPlayer; // VideoPlayer component on the plane (optional)

    private bool isTVOn = false;

    void Start()
    {
        // Debug.Log($"TVToggleController initialized tvPlane: {tvPlane?.name ?? "null"} toggleButton: {toggleButton?.name ?? "null"} videoPlayer: {videoPlayer?.name ?? "null"}");
        // // Make sure the TV starts hidden
        // if (tvPlane != null)
        //     tvPlane.SetActive(false);

        // Wire up the button click
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleTV);
    }

    public void ToggleTV()
    {
        Debug.Log("ToggleTV called");
        isTVOn = !isTVOn;

        if (tvPlane != null)
            // tvPlane.SetActive(isTVOn);
            Debug.Log($"TV plane set to {(isTVOn ? "active" : "inactive")}");

        if (videoPlayer != null)
        {
            if (isTVOn){
                videoPlayer.Play();
                Debug.Log("VideoPlayer started playing");}
            else
                {videoPlayer.Pause(); // pauses rather than stops, so it resumes from the same point next time
                Debug.Log("VideoPlayer paused");}
        }
    }
}