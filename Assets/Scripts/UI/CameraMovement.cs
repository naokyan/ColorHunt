using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.05f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.backgroundColor = Color.black;

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");

    }

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

    }

}
