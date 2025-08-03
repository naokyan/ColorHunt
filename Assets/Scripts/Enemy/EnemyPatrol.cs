using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private LayerMask wallLayer; // Layer for wall or platform edge detection
    [SerializeField] private LayerMask voidLayer; // Layer for patrollable void tiles
    [SerializeField] private LayerMask lightRingLayer; // Layer for outer light ring
    [SerializeField] private LayerMask lightShadesLayer; // Layer for inner light shade

    private bool movingRight = true;

    [SerializeField] private Transform groundCheck; // Point to check for ground or void
    [SerializeField] private Transform wallCheck;   // Point to check for wall ahead
    [SerializeField] private Transform player;

    private Rigidbody2D rb;
    private bool enemyWasInsideDome = false;
    private bool enemyInsideDome;

    [SerializeField] private bool patrolVoid; // Whether to patrol on void instead of wall
    private ColorControl cc;

    private float flipCooldown = 0.2f; // Delay between flips
    private float flipTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = FindObjectOfType<ColorControl>();
    }

    void Update()
    {
        flipTimer -= Time.deltaTime;

        Patrol();

        // Track whether enemy just entered/exited the dome
        bool currentlyInsideDome = hittingShades();
        if (currentlyInsideDome != enemyWasInsideDome)
        {
            enemyInsideDome = currentlyInsideDome;

            // Count how many enemies entered the dome
            if (enemyInsideDome)
            {
                int enemyTrappedCount = PlayerPrefs.GetInt("EnemyTrappedCount", 0);
                PlayerPrefs.SetInt("EnemyTrappedCount", enemyTrappedCount + 1);
                PlayerPrefs.Save();
            }

            enemyWasInsideDome = currentlyInsideDome;
        }
    }

    private void Patrol()
    {
        // Move horizontally based on direction
        rb.velocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.velocity.y);

        // Flip direction when reaching an edge or obstacle
        if (!patrolVoid)
        {
            if (flipTimer <= 0f && atGroundEdge() || hittingWall() || (hittingRing() && !hittingShades()))
            {
                Flip();
            }
        }
        else
        {
            if (flipTimer <= 0f && atVoidEdge() || hittingWall() || (hittingRing() && !hittingShades()))
            {
                Flip();
            }
        }
    }

    // Check if there's no void beneath
    private bool atVoidEdge()
    {
        return !Physics2D.OverlapCircle(groundCheck.position, 0.1f, voidLayer);
    }

    // Check if there's no wall/platform beneath
    private bool atGroundEdge()
    {
        return !Physics2D.OverlapCircle(groundCheck.position, 0.1f, wallLayer);
    }

    // Check if hitting wall in front
    private bool hittingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    // Check if standing on the light ring
    private bool hittingRing()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, lightRingLayer);
    }

    // Check if standing on the inner dome (light shade)
    private bool hittingShades()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, lightShadesLayer);
    }

    // Flip direction when bumping into another enemy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Flip();
        }
    }

    // Flip enemy direction and sprite
    private void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
        flipTimer = flipCooldown;
    }
}
