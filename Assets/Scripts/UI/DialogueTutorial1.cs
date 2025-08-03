using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;

public class DialogueTutorial1 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject uiImage;

    private int index;
    private PlayerMovement pm;
    private GameManager gm;

    private bool moved;
    private bool jumped;

    private bool aPressed = false;
    private bool dPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        gm = FindObjectOfType<GameManager>();
        textComponent.text = string.Empty;
        if (uiImage != null) uiImage.SetActive(false);
        StartCoroutine(StartDialogueWithDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (textComponent.text == lines[index])
        {
            if (moved && index < 4)
            {
                NextLine();
            }
            if (jumped)
            {
                NextLine();
            }
        }


        if (!gm.isPaused && index == 2)
        {
            if (!moved)
            {
                pm.canMove = true;
                if (Input.GetKeyUp(KeyCode.A))
                {
                    aPressed = true;
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    dPressed = true;
                }
                if (aPressed && dPressed)
                {
                    moved = true;
                    
                }
            }

            
        }

        if (!jumped && index == 4)
        {
            pm.canJump = true;
            if (Input.GetKeyUp(KeyCode.J) || Input.GetButtonDown("Jump"))
            {
                jumped = true;
            }
        }


    }

    IEnumerator StartDialogueWithDelay()
    {
        yield return new WaitForSeconds(2f); // 2-second delay
        if (uiImage != null) uiImage.SetActive(true); // Show the UI image
        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(1f);
        if (index < 2)
        {
              // Optional pause before advancing
            NextLine();
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.transform.parent.Find("Image").gameObject.SetActive(false);
            gameObject.SetActive(false); // Hide dialogue when finished
        }
    }
}
