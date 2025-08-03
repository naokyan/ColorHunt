using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class WallJumpNotice : MonoBehaviour
{
    private GameObject dialogueBox;
    private GameObject text;

    private int index;
    [SerializeField] private Image imageToBeChanged;
    private Color startColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1f);

    void Start()
    {
        dialogueBox = transform.Find("DialogueCanvas").gameObject;
        if (dialogueBox) dialogueBox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueBox.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueBox.SetActive(false);
            if (imageToBeChanged)
            {
                imageToBeChanged.color = startColor;
            }
        }
    }
}
