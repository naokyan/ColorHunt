using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalInput;
    private float speed = 8f;
    private float jumpingPower = 16f;
    public bool isFacingRight = true;

    public bool canMove;
    public bool canJump;
    private bool doubleJump;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private BoxCollider2D boxCollider;
    public Vector3 respawnPosition;
    [SerializeField] private Transform startPosition;

    private Camera mainCamera;

    public bool isGrabbing = false;
    // For FirebaseManager
    private FirebaseManager fm;
    private int deathCount;

    public bool reachCheck;

    private Dictionary<GameObject, bool> obstacleStates = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> enemyStates = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, Vector3> lightPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, bool> lightStatus = new Dictionary<GameObject, bool>();

    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Cursor.visible = false;
        //fm = FindObjectOfType<FirebaseManager>();
        respawnPosition = transform.position;
        // Load deathCount from PlayerPrefs
        deathCount = PlayerPrefs.GetInt("DeathCount", 0);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Obstacle"))
            {
                obstacleStates[obj] = obj.activeInHierarchy;
            }
            if (obj.CompareTag("Enemy"))
            {
                enemyStates[obj] = obj.activeInHierarchy;
            }
            if (obj.CompareTag("Light"))
            {
                lightPositions[obj] = obj.transform.position;
                lightStatus[obj] = obj.transform.GetComponent<LightParent>().lighted;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            animator.SetBool("isWalled", isWalled());

            if (isGrounded())
            {
                doubleJump = true;
                animator.SetBool("isJumping", false); // grounded = not jumping
            }
            else
            {
                animator.SetBool("isJumping", true); // in air = jumping
            }

            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput != 0)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }

            bool grounded = isGrounded();
            bool noHorizontal = Mathf.Abs(horizontalInput) < 0.01f;

            bool isFreeFalling = !grounded && noHorizontal && !isWallSliding;
            animator.SetBool("isFreeFalling", isFreeFalling);

            if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J)) && canJump)
            {
                if (isGrounded())
                {
                    // First jump
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    doubleJump = true;
                }
                else if (doubleJump)
                {
                    // Double jump
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    doubleJump = false;
                }
            }

            /*
            if ((Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.J)) && rb.velocity.y > 0f && canJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }*/

            WallSlide();
            WallJump();

            if (!isWallJumping)
            {
                Flip();
            }

        }
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        }

    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, wallLayer) ||
               Physics2D.OverlapCircle(groundCheck2.position, 0.2f, wallLayer);
    }


    private void WallSlide()
    {
        if (isWalled() && !isGrounded() && horizontalInput != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
            isWallSliding = false;

        animator.SetBool("isWallSliding", isWallSliding);
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J)) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Including death
        {
            // Increase death count and save it to PlayerPrefs
            deathCount++;
            PlayerPrefs.SetInt("DeathCount", deathCount);
            PlayerPrefs.Save();

            string savedStringForDeathLocation = PlayerPrefs.GetString("DeathLocation", "0");
            PlayerPrefs.SetString("DeathLocation", getCurrentPos().ToString() + ";" + savedStringForDeathLocation);
            PlayerPrefs.Save();

            Respawn();

            // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the scene
        }
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        rb.velocity = Vector2.zero;

        foreach (var obstacle in obstacleStates)
        {
            if (obstacle.Key != null)
            {
                obstacle.Key.SetActive(obstacle.Value);
            }
        }

        foreach (var enemy in enemyStates)
        {
            if (enemy.Key != null)
            {
                enemy.Key.SetActive(enemy.Value);
            }
        }

        foreach (var light in lightPositions)
        {
            if (light.Key != null)
            {
                light.Key.transform.position = light.Value;
                light.Key.transform.parent = null;
            }
        }

        foreach (var light in lightStatus)
        {
            if (light.Key != null)
            {
                LightParent lightParent = light.Key.GetComponent<LightParent>();
                if (lightParent != null)
                {
                    lightParent.lighted = light.Value;
                }
            }
        }
    }


    public void SaveGameState()
    {
        // Clear existing data
        obstacleStates.Clear();
        enemyStates.Clear();
        lightPositions.Clear();
        lightStatus.Clear();

        // Find all current game objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Obstacle"))
            {
                obstacleStates[obj] = obj.activeInHierarchy;
            }
            if (obj.CompareTag("Enemy"))
            {
                enemyStates[obj] = obj.activeInHierarchy;
            }
            if (obj.CompareTag("Light"))
            {
                lightPositions[obj] = obj.transform.position;
                LightParent lightParent = obj.GetComponent<LightParent>();
                if (lightParent != null)
                {
                    lightStatus[obj] = lightParent.lighted;
                }
            }
        }
    }



    public Vector2 getCurrentPos()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}