using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PressAnimation : MonoBehaviour
{
    private Image targetImage;

    private Color startColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1f); 
    private Color endColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f); 
    private float timer = 0f;
    private float switchInterval = 0.5f;
    private bool isStartColor = true;
    
    [SerializeField] private bool toggleOnce;

    private void Start()
    {
        // Get the Image component on the same GameObject
        targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        // Only change color if the targetImage and GameObject are active
        if (targetImage != null && gameObject.activeInHierarchy)
        {

            if (!toggleOnce)
            {
                timer += Time.deltaTime;

                // Check if the switch interval has been reached
                if (timer >= switchInterval)
                {
                    // Switch colors
                    targetImage.color = isStartColor ? endColor : startColor;
                    isStartColor = !isStartColor;
                    timer = 0f; // Reset the timer
                }
            }
            else
            {
                timer += Time.deltaTime;

                // Check if the switch interval has been reached
                if (timer >= switchInterval)
                {
                    targetImage.color = endColor;
                    timer = 0f;
                }
            }

        }
    }
}
