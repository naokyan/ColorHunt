using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    private RectTransform arrowRectTransform; // The arrow's RectTransform component
    private Camera mainCamera;
    private GameObject arrowImage;
    private Transform target;

    public void SetupArrow(Transform target, Camera camera)
    {
        this.target = target;
        this.mainCamera = camera;

        arrowImage = transform.Find("ArrowImage").gameObject;
        arrowRectTransform = arrowImage.GetComponent<RectTransform>();
        arrowImage.SetActive(true);

        if (target.CompareTag("Goal"))
        {
            arrowImage.GetComponent<Image>().color = target.GetComponent<SpriteRenderer>().color;
        }
        else if (target.Find("LightSwitch") != null)
        {
            arrowImage.GetComponent<Image>().color = target.Find("LightSwitch").gameObject.GetComponent<SpriteRenderer>().color;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(target.position);
        bool isTargetInside = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        Vector3 direction = target.position - mainCamera.transform.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        if (isTargetInside)
        {
            arrowImage.SetActive(false); // Hide the arrow if the target is within the camera view
        }
        else
        {
            arrowImage.SetActive(true);

            screenPoint = mainCamera.WorldToScreenPoint(target.position);
            screenPoint.z = 0;

            // Clamp the arrow's position to keep it on the edge of the screen
            float clampedX = Mathf.Clamp(screenPoint.x, 50, Screen.width - 50);
            float clampedY = Mathf.Clamp(screenPoint.y, 50, Screen.height - 50);

            // Define a threshold range for the top-left corner
            float cornerThreshold = 90f;

            // Adjust position if arrow is near the top-left corner
            if (clampedX <= 50 + cornerThreshold && clampedY >= Screen.height - 50 - cornerThreshold)
            {
                clampedX += cornerThreshold; // Move right by the threshold value
                clampedY -= cornerThreshold; // Move down by the threshold value
            }

            arrowRectTransform.position = new Vector3(clampedX, clampedY, 0);
        }
    }


}