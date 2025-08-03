using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PressAnimationTrack : MonoBehaviour
{
    public Transform target; // The target object
    public GameObject arrow;
    public GameObject pressK;
    public GameObject pressL;

    private GameObject pressButton;

    private float offsetAboveTarget = 1.2f; // Distance above the target object
    private bool isStartColor = true;
    private float timer = 0f;
    private float switchInterval = 0.5f;

    private Color arrowStartColor = new Color(1f, 1f, 1f, 1f);
    private Color buttonStartColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1f);
    private Color endColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

    private bool howToToggleLight;

    void Start()
    {
        arrow.SetActive(true);
        pressButton = pressK;
        pressButton.SetActive(true);
    }

    void Update()
    {
        if (target.parent != null)
        {
            pressButton.SetActive(false);
            arrow.SetActive(false);
        }
        else
        {
            if (!howToToggleLight)
            {
                pressButton.SetActive(true);
                pressButton = pressK;
                pressK.SetActive(true);
                pressL.SetActive(false);
                arrow.SetActive(true);
            }
        }

        if (howToToggleLight)
        {
            if (target.GetComponent<LightParent>())
            {
                if (target.GetComponent<LightParent>().lighted)
                {
                    pressButton.SetActive(false);
                    arrow.SetActive(false);
                }
                else
                {
                    pressButton.SetActive(true);
                    pressButton = pressL;
                    pressK.SetActive(false);
                    pressL.SetActive(true);
                    arrow.SetActive(true);
                }
            }
            
        }


        if (target != null && arrow.activeInHierarchy)
        {
            // Position the arrow and button above the target
            arrow.transform.position = new Vector3(target.position.x, target.position.y + offsetAboveTarget, target.position.z);
            pressButton.transform.position = new Vector3(target.position.x, target.position.y + offsetAboveTarget + 1.2f, target.position.z);

            timer += Time.deltaTime;

            // Switch colors at intervals
            if (timer >= switchInterval)
            {
                arrow.GetComponent<SpriteRenderer>().color = isStartColor ? endColor : arrowStartColor;
                pressButton.GetComponent<SpriteRenderer>().color = isStartColor ? endColor : buttonStartColor;

                isStartColor = !isStartColor;
                timer = 0f; // Reset the timer
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LightSwitch")
        {
            if ((CompareTag("RedLightNotice") && collision.gameObject.transform.parent.gameObject.name == "RedLight")
            || (CompareTag("GreenLightNotice") && collision.gameObject.transform.parent.gameObject.name == "GreenLight"))
            {
                howToToggleLight = true;
            }
        }
        

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LightSwitch")
        {
            if ((CompareTag("RedLightNotice") && collision.gameObject.transform.parent.gameObject.name == "RedLight")
            || (CompareTag("GreenLightNotice") && collision.gameObject.transform.parent.gameObject.name == "GreenLight"))
            {
                howToToggleLight = false;
            }
        }
    }
}



