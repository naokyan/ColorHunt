using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowManager : MonoBehaviour
{
    public GameObject arrowPrefab; // Prefab of the arrow with the Arrow script attached
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Find all GameObjects with tags "Goal" or "Light"
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("Goal"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("Light"));

        // Instantiate an arrow for each target
        foreach (GameObject target in targets)
        {
            GameObject arrowInstance = Instantiate(arrowPrefab, transform);
            Arrow arrowScript = arrowInstance.GetComponent<Arrow>();
            arrowScript.SetupArrow(target.transform, mainCamera);
        }
    }
}


