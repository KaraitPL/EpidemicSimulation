using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dot : MonoBehaviour
{
    public float healthySpeed = 1.0f;
    public float infectedSpeed = 0.5f;
    private SpriteRenderer spriteRenderer;
    public float infectionDeathRate = 3f;
    public float infectionRate = 2f;
    public float reInfectionRate = 1.5f;
    public float recoveryRate = 0.5f;
    public float symptomsRate = 0.4f;
    public float moveSpeed = 0.2f;
    public bool isChanging = false;
    public float maxLifetime = 100f;
    private int liveTime;
    public float DeltaTime;
    float[] deathHealthyProb = { 0.00417f, 0.00032f, 0.00023f, 0.00018f, 0.00015f, 0.00012f, 0.0001f, 0.00009f, 0.00009f, 0.00009f, 0.00009f, 0.0001f, 0.00012f, 0.00015f, 0.00019f, 0.00025f, 0.00033f, 0.00042f, 0.00053f, 0.00063f, 0.00073f, 0.00081f, 0.00088f, 0.00094f, 0.00099f, 0.00105f, 0.00111f, 0.00118f, 0.00125f, 0.00133f, 0.00142f, 0.00152f, 0.00163f, 0.00176f, 0.0019f, 0.00205f, 0.00222f, 0.0024f, 0.00259f, 0.0028f, 0.00303f, 0.00329f, 0.00358f, 0.00391f, 0.00428f, 0.00468f, 0.00513f, 0.00562f, 0.00617f, 0.00678f, 0.00746f, 0.00821f, 0.00902f, 0.0099f, 0.01084f, 0.01185f, 0.01296f, 0.01418f, 0.01552f, 0.01702f, 0.01868f, 0.0205f, 0.02248f, 0.02465f, 0.02698f, 0.02943f, 0.03197f, 0.0346f, 0.03724f, 0.03996f, 0.04278f, 0.04581f, 0.0491f, 0.05275f, 0.05677f, 0.06123f, 0.06608f, 0.07135f, 0.07707f, 0.08337f, 0.09033f, 0.09817f, 0.10707f, 0.11713f, 0.12822f, 0.14025f, 0.15297f, 0.16613f, 0.1795f, 0.1931f, 0.20692f, 0.22099f, 0.23536f, 0.25003f, 0.26493f, 0.27999f, 0.29514f, 0.31029f, 0.32536f, 0.34029f, 0.355f };
    float[] deathInfectedProb = new float[100];
    float[] infectionProb = new float[100];
    float[] reInfectionProb = new float[100];
    float[] recoveryProb = new float[100];
    float[] symptomsProb = new float[100];
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < 100; i++)
        {
            deathInfectedProb[i] = infectionDeathRate * deathHealthyProb[i];
            infectionProb[i] = infectionRate * deathHealthyProb[i];
            reInfectionProb[i] = reInfectionRate * deathHealthyProb[i];
            recoveryProb[i] = recoveryRate * (1 - deathHealthyProb[i]);
            symptomsProb[i] = symptomsRate * deathHealthyProb[i];
        }
        liveTime = Random.Range(0, 70);
        
        setColor(Color.yellow);
    }

    private void Update()
    {
        DeltaTime += Time.deltaTime;

        if (DeltaTime > (float)maxLifetime / 50)
        {
            updateColor();


            if (gameObject.tag == "Infected")
            {
                if ((float)Random.Range(0, 100000) / 100000 <= symptomsProb[liveTime])
                    gameObject.tag = "SInfected";
            }

            

            if(gameObject.tag == "Infected" || gameObject.tag == "SInfected")
            {
                if ((float)Random.Range(0, 100000) / 100000 <= deathInfectedProb[liveTime])
                {
                    gameObject.tag = "Dead";
                }
                else if(gameObject.tag == "SInfected")
                { 
                    if ((float)Random.Range(0, 100000) / 100000 <= recoveryProb[liveTime])
                        gameObject.tag = "Recovered";
                }
            }
            else
            {
                if ((float)Random.Range(0, 100000) / 100000 <= deathHealthyProb[liveTime])
                {
                    gameObject.tag = "Dead";
                }
            }

            liveTime++;
            if (liveTime > 99)
                liveTime = 99;
            DeltaTime = 0;
        }


        if (gameObject.tag == "Infected")
        {
            setColor(Color.magenta);
            moveSpeed = healthySpeed;
        }
        else if (gameObject.tag == "SInfected")
        {
            setColor(Color.red);
            moveSpeed = infectedSpeed;
        }
        else if(gameObject.tag == "Dead")
        {
            setColor(Color.grey);
            moveSpeed = 0;
        }
        else if( gameObject.tag == "Recovered")
        {
            setColor(Color.green);
            moveSpeed = healthySpeed;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.tag != "Dead")
        {
            if (collision.gameObject.tag == "Infected" || collision.gameObject.tag == "SInfected")
            {
                if (gameObject.tag == "Uninfected" && (float)Random.Range(0, 100000) / 100000 <= infectionProb[liveTime] || gameObject.tag == "Recovered" && (float)Random.Range(0, 100000) / 100000 <= reInfectionProb[liveTime])
                {
                    gameObject.tag = "Infected";
                }
            }
        }
    }

    public void setColor(Color col)
    {

        float r = col.r;
        float g = col.g;
        float b = col.b;

        float r1 = (float)liveTime / 200;
        float g1 = (float)liveTime / 200;
        float b1 = (float)liveTime / 200;


        r -= r1;
        g -= g1;
        b -= b1;

        if (r < 0)
            r = 0;
        if (g < 0)
            g = 0;
        if (b < 0)
            b = 0;

        Color newColor = new Color(r, g, b, col.a);
        spriteRenderer.color = newColor;
    }

    public void updateColor()
    {
        Color col = spriteRenderer.color;

        float r = col.r;
        float g = col.g;
        float b = col.b;

        r -= 0.01f;
        g -= 0.01f;
        b -= 0.01f;

        if (r < 0)
            r = 0;
        if (g < 0)
            g = 0;
        if (b < 0)
            b = 0;

        Color newColor = new Color(r, g, b, col.a);
        spriteRenderer.color = newColor;
    }


}
