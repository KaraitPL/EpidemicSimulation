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
    public float defaultMoveSpeed = 0.2f;
    private SpriteRenderer spriteRenderer;

    private float infectionDeathRate;
    private float infectionRate;
    private float reInfectionRate;
    private float recoveryRate;
    private float symptomsRate;

    private float yearOfSimulation = 0;
    private float moveSpeed = 0.2f;
    private float timeMultiplier;
    public bool isChanging = false;
    private float yearLength = 4f; //D³ugoœæ trwania roku
    private float actionLength = 1f; //Co ile sekund wykonywana jest akcja œmierci/zara¿enia/wyleczenia
    private int liveTime; //Zmienine z private na int, ¿eby InfectionZone Script widzia³
    private float yearDeltaTime = 0;
    private float actionDeltaTime = 0;
    public GameObject infectionZone; //Obiekt strefy wokó³ osoby
    float[] probabilityByAge = new float[100]; //Tablica przedstawiaj¹ca szanse w zale¿noœci od wieku
    float[] deathHealthyProb = { 0.00417f, 0.00032f, 0.00023f, 0.00018f, 0.00015f, 0.00012f, 0.0001f, 0.00009f, 0.00009f, 0.00009f, 0.00009f, 0.0001f, 0.00012f, 0.00015f, 0.00019f, 0.00025f, 0.00033f, 0.00042f, 0.00053f, 0.00063f, 0.00073f, 0.00081f, 0.00088f, 0.00094f, 0.00099f, 0.00105f, 0.00111f, 0.00118f, 0.00125f, 0.00133f, 0.00142f, 0.00152f, 0.00163f, 0.00176f, 0.0019f, 0.00205f, 0.00222f, 0.0024f, 0.00259f, 0.0028f, 0.00303f, 0.00329f, 0.00358f, 0.00391f, 0.00428f, 0.00468f, 0.00513f, 0.00562f, 0.00617f, 0.00678f, 0.00746f, 0.00821f, 0.00902f, 0.0099f, 0.01084f, 0.01185f, 0.01296f, 0.01418f, 0.01552f, 0.01702f, 0.01868f, 0.0205f, 0.02248f, 0.02465f, 0.02698f, 0.02943f, 0.03197f, 0.0346f, 0.03724f, 0.03996f, 0.04278f, 0.04581f, 0.0491f, 0.05275f, 0.05677f, 0.06123f, 0.06608f, 0.07135f, 0.07707f, 0.08337f, 0.09033f, 0.09817f, 0.10707f, 0.11713f, 0.12822f, 0.14025f, 0.15297f, 0.16613f, 0.1795f, 0.1931f, 0.20692f, 0.22099f, 0.23536f, 0.25003f, 0.26493f, 0.27999f, 0.29514f, 0.31029f, 0.32536f, 0.34029f, 0.355f };
    float[] deathInfectedProb = new float[100];
    public float[] infectionProb = new float[100]; //Zmienine z private na int, ¿eby InfectionZone Script widzia³
    public float[] reInfectionProb = new float[100]; //Zmienine z private na int, ¿eby InfectionZone Script widzia³
    float[] recoveryProb = new float[100];
    float[] symptomsProb = new float[100];
    public GameObject area;
    private Area areaScript;
    private void Start()
    {
        Random.InitState(5);
        spriteRenderer = GetComponent<SpriteRenderer>();
        areaScript = area.GetComponent<Area>();

        //Pobieranie wartoœci parametrów modyfikowanych w skrypcie Area
        infectionDeathRate = areaScript.GetInfectionDeathRate();
        infectionRate = areaScript.GetInfectionRate();
        reInfectionRate = areaScript.GetReInfectionRate();
        recoveryRate = areaScript.GetRecoveryRate();
        symptomsRate = areaScript.GetSymptomsRate();


        timeMultiplier = areaScript.timeMultiplier;
        actionLength = actionLength / timeMultiplier;
        yearLength = yearLength / timeMultiplier;
        defaultMoveSpeed = defaultMoveSpeed * timeMultiplier;

        moveSpeed = defaultMoveSpeed;
        SetProbabilityByAgeArray();
        for (int i = 0; i < 100; i++)
        {
            deathInfectedProb[i] = infectionDeathRate * probabilityByAge[i];// * deathHealthyProb[i];
            infectionProb[i] = infectionRate * probabilityByAge[i];// * deathHealthyProb[i];
            reInfectionProb[i] = reInfectionRate * probabilityByAge[i];// * deathHealthyProb[i];
            recoveryProb[i] = recoveryRate * (1 - probabilityByAge[i]);// * (1 - deathHealthyProb[i]);
            symptomsProb[i] = symptomsRate * probabilityByAge[i];// * deathHealthyProb[i];
        }
        liveTime = Random.Range(0, 40);
        
        setColor(Color.yellow);
    }

    private void FixedUpdate()
    {
        if (yearOfSimulation < 10)
        {
            yearDeltaTime += Time.deltaTime;
            actionDeltaTime += Time.deltaTime;
            infectionZone.tag = this.tag;
            if (yearDeltaTime > yearLength) //Po czasie roku obiekt starzeje siê o rok
            {
                updateColor();
                liveTime++;
                if (liveTime > 99)
                    liveTime = 99;
                yearOfSimulation++;
                yearDeltaTime = 0;
            }

            if (actionDeltaTime > actionLength) //Po ustalonym czasie podejmowana jest próba zara¿enia/œmierci/wyleczenia
            {

                if (gameObject.tag == "Infected" || gameObject.tag == "SInfected")
                {
                    if ((float)Random.Range(0, 100000) / 100000 <= deathInfectedProb[liveTime])
                    {
                        gameObject.tag = "Dead";
                    }
                    else if (gameObject.tag == "SInfected")
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

                if (gameObject.tag == "Infected")
                {
                    if ((float)Random.Range(0, 100000) / 100000 <= symptomsProb[liveTime])
                        gameObject.tag = "SInfected";
                }

                actionDeltaTime = 0;
            }


            if (gameObject.tag == "Infected")
            {
                setColor(Color.magenta);
                moveSpeed = defaultMoveSpeed * healthySpeed;
            }
            else if (gameObject.tag == "SInfected")
            {
                setColor(Color.red);
                moveSpeed = defaultMoveSpeed * infectedSpeed;
            }
            else if (gameObject.tag == "Dead")
            {
                setColor(Color.grey);
                moveSpeed = 0;
            }
            else if (gameObject.tag == "Recovered")
            {
                setColor(Color.green);
                moveSpeed = defaultMoveSpeed;
            }
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

    private void SetProbabilityByAgeArray()
    {
        float probability = 0.5f;
        for(int i = 0; i <= 10; i++) //Dla wieku 0-10 (szansa od 50% do 20%)
        {
            probabilityByAge[i] = probability;
            probability -= 0.03f;
        }
        for(int i = 11; i <= 25; i++) //Dla wieku 11-25 (szansa 20%)
        {
            probabilityByAge[i] = probability;
        }
        for(int i = 26; i < 100; i++) //Dla wieku 26-99 (szansa od 21% do 95%)
        {
            probability += 0.01f;
            probabilityByAge[i] = probability;
        }
    }

    public int GetLiveTime() { return liveTime; }

    public float GetMoveSpeed() { return moveSpeed; }

    public float GetActionLength() { return actionLength; }

    public float GetYearOfSimulation() { return yearOfSimulation; }




}
