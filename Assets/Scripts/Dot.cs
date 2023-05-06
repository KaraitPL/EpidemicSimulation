using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dot : MonoBehaviour
{
    public float healthySpeed = 1.0f;
    public float infectedSpeed = 0.5f;
    private SpriteRenderer spriteRenderer;
    public float infectionProb = 0.5f;
    public float reInfectionProb = 0.2f;
    public float recoveryProb = 0.5f;
    public float symptomsProb = 0.5f;
    public float moveSpeed = 0.2f;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Random.InitState(42);
    }

    private void Update()
    {

        if(gameObject.tag == "Infected")
        {
            spriteRenderer.color = Color.magenta;
            moveSpeed = healthySpeed;
        }
        else if(gameObject.tag == "SInfected")
        {
            spriteRenderer.color = Color.red;
            moveSpeed = infectedSpeed;
        }

        if (gameObject.tag == "Infected" || gameObject.tag == "SInfected")
        {
            if ((float)Random.Range(0, 100) / 100 <= recoveryProb)
                StartCoroutine(Recover());
            else
                StartCoroutine(Die());
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.tag != "Dead")
        {
            if (collision.gameObject.tag == "Infected" || collision.gameObject.tag == "SInfected")
            {
                if (gameObject.tag == "Uninfected"  && (float)Random.Range(0, 100) / 100 <= infectionProb || gameObject.tag == "Recovered" && (float)Random.Range(0, 100) / 100 <= reInfectionProb)
                {
                    if ((float)Random.Range(0, 100)/100 <= symptomsProb)
                        gameObject.tag = "SInfected";
                    else
                        gameObject.tag = "Infected";
                }
            }
        }
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(3f);
        gameObject.tag = "Recovered";
        moveSpeed = healthySpeed;
        spriteRenderer.color = Color.green;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(3f);
        gameObject.tag = "Dead";
        moveSpeed = 0;
        spriteRenderer.color = Color.gray;
    }

    public float getMoveSpeed()
    {
        return moveSpeed;
    }

    
}
