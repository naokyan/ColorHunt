using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class MovingWithEnemy : MonoBehaviour
{
    [SerializeField] private Transform enemy;               
    [SerializeField] private Vector3 positionOffset;         

    void LateUpdate()
    {
        if (enemy != null)
        {
            transform.position = enemy.position + positionOffset;
        }
    }
}
