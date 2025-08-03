using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private PlayerMovement pm;
    private Sprite originalCheckpoint;

    private GameObject checkpointLight;

    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        originalCheckpoint = GetComponent<SpriteRenderer>().sprite;

        // Find and disable the "light" child object
        checkpointLight = transform.Find("light")?.gameObject;
        if (checkpointLight != null)
        {
            checkpointLight.SetActive(false);
        }
    }

    void Update()
    {
        Vector3 respawn = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        if (pm.reachCheck && pm.respawnPosition != respawn)
        {
            if (checkpointLight != null)
            {
                checkpointLight.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pm.reachCheck = true;
            Vector3 checkpointPosition = transform.position;
            pm.respawnPosition = new Vector3(checkpointPosition.x, checkpointPosition.y + 1.5f, checkpointPosition.z);

            if (checkpointLight != null)
            {
                checkpointLight.SetActive(true);
            }

            pm.SaveGameState();
        }
    }
}
