using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [Header("Flashing Image Settings")]
    public Image pressAnyButtonImage; // The Image component of "Press Any Button"
    public float flashDuration = 1f;  // Time it takes to fade in or out

    [Header("Screen Fade Settings")]
    public Image blackOverlayImage;   // The full-screen black Image for fading
    public float fadeDuration = 1f;   // Duration of the fade to black
    public string nextSceneName;      // Name of the next scene to load

    private float flashTimer = 0f;
    private bool fadingOut = true;
    private bool inputDetected = false;
    private float fadeTimer = 0f;
    private bool isFading = false;

    void Start()
    {
        // Initialize the alpha of the black overlay to 0 (fully transparent)
        if (blackOverlayImage != null)
        {
            Color color = blackOverlayImage.color;
            color.a = 0f;
            blackOverlayImage.color = color;
        }
    }

    void Update()
    {
        // If input has not been detected yet, handle the flashing image and input detection
        if (!inputDetected)
        {
            HandleFlashingImage();
            DetectInput();
        }
        else if (isFading)
        {
            // Handle the fade to black
            HandleScreenFade();
        }
    }

    void HandleFlashingImage()
    {
        if (pressAnyButtonImage == null)
            return;

        // Update the timer
        flashTimer += Time.deltaTime;

        // Calculate the alpha value
        float alpha;
        if (fadingOut)
        {
            // Fading out: alpha goes from 1 to 0
            alpha = Mathf.Lerp(1f, 0f, flashTimer / flashDuration);
        }
        else
        {
            // Fading in: alpha goes from 0 to 1
            alpha = Mathf.Lerp(0f, 1f, flashTimer / flashDuration);
        }

        // Apply the alpha value to the image
        Color color = pressAnyButtonImage.color;
        color.a = alpha;
        pressAnyButtonImage.color = color;

        // Switch fading direction after the duration has elapsed
        if (flashTimer >= flashDuration)
        {
            flashTimer = 0f;
            fadingOut = !fadingOut;
        }
    }

    void DetectInput()
    {
        // Detect any key or button press
        if (Input.anyKeyDown)
        {
            inputDetected = true;
            isFading = true;
            fadeTimer = 0f;

            // Optionally, disable the "Press Any Button" image
            if (pressAnyButtonImage != null)
            {
                pressAnyButtonImage.gameObject.SetActive(false);
            }
        }
    }

    void HandleScreenFade()
    {
        if (blackOverlayImage == null)
            return;

        fadeTimer += Time.deltaTime;
        float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

        // Update the black overlay's alpha
        Color color = blackOverlayImage.color;
        color.a = alpha;
        blackOverlayImage.color = color;

        // Check if fade is complete
        if (fadeTimer >= fadeDuration)
        {
            isFading = false;
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
