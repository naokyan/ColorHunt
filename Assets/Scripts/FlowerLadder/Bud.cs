using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bud : MonoBehaviour
{
    private Flower parentScript;
    private ColorControl cc;

    void Start()
    {
        parentScript = transform.parent.GetComponent<Flower>();
        cc = FindObjectOfType<ColorControl>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LightShades") || collision.CompareTag("LightRing"))
        {
            Color objectColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            if (cc.CompareColors(objectColor, GetComponent<SpriteRenderer>().color))
            {
                parentScript.isBlossom = true;
            }
        }
    }
}
