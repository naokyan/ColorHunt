using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public bool isBlossom;

    private GameObject Bud;
    private GameObject Ladder;

    void Start()
    {
        Bud = transform.Find("Bud").gameObject;
        Ladder = transform.Find("Ladder").gameObject;
        Bud.SetActive(true);
        Ladder.SetActive(false);
    }

    void Update()
    {
        if (isBlossom)
        {
            Bud.SetActive(false);
            Ladder.SetActive(true);
        }
    }
}
