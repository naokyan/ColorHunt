using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Video;

public class PlayerDemoVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject demo;
    private PlayerMovement pm;
    [SerializeField] private GameObject helpText;

    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        Time.timeScale = 0f;
        Cursor.visible = true;
        pm.enabled = false;
        videoPlayer.isLooping = true;
        helpText.SetActive(false);
        demo.SetActive(true);
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "demoUnity.mp4");
        videoPlayer.loopPointReached += OnVideoEnd;
    }


    private void OnVideoEnd(VideoPlayer vp)
    {
        vp.time = 0; // Reset video to start
        vp.Play();   // Play again
    }

    public void EndVideo()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        demo.SetActive(false);
        helpText.SetActive(true);
        pm.enabled = true;
    }

    public void SkipButton()
    {
        EndVideo();
    }
}
