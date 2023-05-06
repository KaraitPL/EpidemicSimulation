using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int immunity;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Random.InitState(42);
    }

    private void Update()
    {
        if(gameObject.tag == "Infected")
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(Die());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.tag != "Dead")
        {
            if (collision.gameObject.tag == "Infected")
            {
                int random = Random.Range(0, 100);
                if (random >= immunity)
                {
                    gameObject.tag = "Infected";
                }
            }
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(3f);
        gameObject.tag = "Dead";
        spriteRenderer.color = Color.gray;
    }

    
}
