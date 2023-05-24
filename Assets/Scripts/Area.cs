using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class Area : MonoBehaviour
{
    public float timeMultiplier = 1;
    public GameObject notInfected;
    public const int numberOfDots = 200;
    public float areaWidth = 5f;
    public float areaHeight = 5f;
    public int numberOfInfected;
    private GameObject[] dots = new GameObject[numberOfDots];

    private string filePath = "C:\\DaneEpidemia\\daneNowe212.csv";
    private StreamWriter writer;

    private NumberFormatInfo nfi;

    private float currentYear;

    private List<WorldState> stateEveryYear;
    private List<DotState[]> dotsStateEveryYear; //Lista tablic zawieracj¹cych stan wszystkich kropek

    //private static float infectionDeathRate = 0.04f;
    //private static float infectionRate = 0.2f;
    //private static float reInfectionRate = 0.1f;
    //private static float recoveryRate = 0.2f;
    //private static float symptomsRate = 0.3f;

    private static float infectionDeathRate = 0.05f;
    private static float infectionRate = 0.05f;
    private static float reInfectionRate = 0.05f;
    private static float recoveryRate = 0.05f;
    private static float symptomsRate = 0.05f;

    public struct WorldState
    {
        private short numberOfNotinfected;
        private short numberOfInfected;
        private short numberOfDead;
        private short numberOfRecovered;
        public WorldState(short numberOfNotinfected, short numberOfInfected, short numberOfDead, short numberOfRecovered)
        {
            this.numberOfNotinfected = numberOfNotinfected;
            this.numberOfInfected = numberOfInfected;
            this.numberOfDead = numberOfDead;
            this.numberOfRecovered = numberOfRecovered;
        }

        public short GetNotinfected() { return numberOfNotinfected; }
        public short GetInfected() { return numberOfInfected; }
        public int GetDead() { return numberOfDead; }

        public int GetRecovered() { return numberOfRecovered; }

        public override string ToString()
        {
            return "Notinfected: " + numberOfNotinfected + " Infected: " + numberOfInfected + " Dead: " + numberOfDead + " Recovered: " + numberOfRecovered;
        }
    }

    public struct DotState
    {
        private float xPos;
        private float yPos;
        private float zRot;
        private short notinfected;
        private short infected;
        private short recovered;
        private short dead;

        public DotState(float xPos, float yPos, float zRot, short a, short b, short c, short d)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.zRot = zRot;
            this.notinfected = a;
            this.infected = b;
            this.recovered = c;
            this.dead = d;
        }

        public float GetxPos()
        {
            return xPos;
        }
        public float GetyPos()
        {
            return yPos;
        }
        public float GetzRot()
        {
            return zRot;
        }
        public short GetNotInfected()
        {
            return notinfected;
        }
        public short GetInfected()
        {
            return infected;
        }
        public short GetRecovered()
        {
            return recovered;
        }
        public short GetDead()
        {
            return dead;
        }

        

    }

    float minX;
    float maxX;
    float minY;
    float maxY;

    void Start()
    {
        nfi = new CultureInfo("en-US", false).NumberFormat;
        writer = new StreamWriter(filePath);
        //writer.WriteLine("InfDeadRate,InfRate,ReInfRate,RecovRate,SympRate,Dead,Infected,Notinfecte,Recovered");


        Random.InitState((int)DateTime.Now.Ticks);
        Initialize();
    }
    void FixedUpdate()
    {
        float year = dots[0].GetComponent<Dot>().GetYearOfSimulation();

        if (year > currentYear)
        {
            currentYear = year;
            CountDifferentDots();
            if (currentYear == 10)
            {
                //PrintStateArray();
                PrintStateOfDots();
                NewSimulation();
            }


        }

        //Debug.Log(countSignal);
        //if (Input.GetKey(KeyCode.Space))
        //{
        if (currentYear < 10)
        {
            for (int i = 0; i < numberOfDots; i++)
            {
                float rotationAngle = Random.Range(-100, 100);
                rotationAngle = rotationAngle / 10;
                dots[i].transform.eulerAngles = dots[i].transform.eulerAngles + new Vector3(0, 0, rotationAngle);

                if (dots[i].transform.eulerAngles.z >= 360)
                    dots[i].transform.eulerAngles = new Vector3(0, 0, 0);

                if (dots[i].transform.eulerAngles.z < 0)
                    dots[i].transform.eulerAngles = new Vector3(0, 0, 359);

                dots[i].transform.position += dots[i].transform.up * dots[i].GetComponent<Dot>().GetMoveSpeed() * 7 * Time.deltaTime;

                float xPos = dots[i].transform.position.x;
                float yPos = dots[i].transform.position.y;
                if (xPos < minX || xPos > maxX || yPos > maxY || yPos < minY)
                {

                    if (xPos < minX)
                    {
                        dots[i].transform.eulerAngles = new Vector3(0, 0, -dots[i].transform.eulerAngles.z);
                        dots[i].transform.position = new Vector3(minX + 0.01f, yPos, 0);
                    }
                    if (xPos > maxX)
                    {
                        dots[i].transform.eulerAngles = new Vector3(0, 0, -dots[i].transform.eulerAngles.z);
                        dots[i].transform.position = new Vector3(maxX - 0.01f, yPos, 0);
                    }
                    if (yPos < minY)
                    {
                        dots[i].transform.eulerAngles = new Vector3(0, 0, 180 - dots[i].transform.eulerAngles.z);
                        dots[i].transform.position = new Vector3(xPos, minY + 0.01f, 0);
                    }
                    if (yPos > maxY)
                    {
                        dots[i].transform.eulerAngles = new Vector3(0, 0, 180 - dots[i].transform.eulerAngles.z);
                        dots[i].transform.position = new Vector3(xPos, maxY - 0.01f, 0);
                    }
                }




            }
        }

        //}
    }

    public void CountDifferentDots()
    {
        short notinfNumber = 0;
        short infNumber = 0;
        short deadNumber = 0;
        short recoverNumber = 0;

        foreach (GameObject dot in dots)
        {
            if (dot.tag == "Uninfected") { notinfNumber++; }
            else if (dot.tag == "Infected" || dot.tag == "SInfected") { infNumber++; }
            else if (dot.tag == "Dead") { deadNumber++; }
            else if (dot.tag == "Recovered") { recoverNumber++; }
        }

        DotState[] dotsArray = new DotState[200];

        for (int i = 0; i < 200; i++)
        {
            short notinf = 0;
            if (dots[i].tag == "Uninfected")
                notinf = 1;

            short inf = 0;
            if (dots[i].tag == "Infected" || dots[i].tag == "SInfected")
                inf = 1;

            short rec = 0;
            if (dots[i].tag == "Recovered")
                rec = 1;

            short dead = 0;
            if (dots[i].tag == "Dead")
                dead = 1;

            dotsArray[i] = new DotState(dots[i].transform.position.x, dots[i].transform.position.y, dots[i].transform.rotation.z, notinf, inf, rec, dead);
        }
        dotsStateEveryYear.Add(dotsArray);
        stateEveryYear.Add(new WorldState(notinfNumber, infNumber, deadNumber, recoverNumber));
    }

    public void PrintStateArray()
    {
        for (int i = 0; i < 10; i++)
        {
            writer.WriteLine(infectionDeathRate.ToString(nfi) + "," + infectionRate.ToString(nfi) + "," + reInfectionRate.ToString(nfi) + "," + recoveryRate.ToString(nfi) + "," +symptomsRate.ToString(nfi) + "," + stateEveryYear[i].GetNotinfected() + "," + stateEveryYear[i].GetInfected() + "," + stateEveryYear[i].GetRecovered() + "," + stateEveryYear[i].GetDead() + "," + stateEveryYear[i + 1].GetNotinfected() + "," + stateEveryYear[i + 1].GetInfected() + "," + stateEveryYear[i + 1].GetRecovered() + "," + stateEveryYear[i + 1].GetDead());
            writer.Flush();
        }
    }

    public void PrintStateOfDots()
    {
        for (int i = 0; i < 10; i++)
        {
            writer.Write(infectionDeathRate.ToString(nfi) + "," + infectionRate.ToString(nfi) + "," + reInfectionRate.ToString(nfi) + "," + recoveryRate.ToString(nfi) + "," + symptomsRate.ToString(nfi));
            for (int j = 0; j < 200; j++)
            {
                writer.Write("," + dotsStateEveryYear[i][j].GetxPos().ToString(nfi) + "," + dotsStateEveryYear[i][j].GetyPos().ToString(nfi) + "," + dotsStateEveryYear[i][j].GetzRot().ToString(nfi) + ","  + dotsStateEveryYear[i][j].GetNotInfected() + "," + dotsStateEveryYear[i][j].GetInfected() + "," + dotsStateEveryYear[i][j].GetRecovered() + "," + dotsStateEveryYear[i][j].GetDead());
            }
            for (int j = 0; j < 200; j++)
            {
                writer.Write("," + dotsStateEveryYear[i+1][j].GetxPos().ToString(nfi) + "," + dotsStateEveryYear[i+1][j].GetyPos().ToString(nfi) + "," + dotsStateEveryYear[i+1][j].GetzRot().ToString(nfi) + "," + dotsStateEveryYear[i + 1][j].GetNotInfected() + "," + dotsStateEveryYear[i + 1][j].GetInfected() + "," + dotsStateEveryYear[i + 1][j].GetRecovered() + "," + dotsStateEveryYear[i + 1][j].GetDead());
            }
            writer.WriteLine();
            writer.Flush();
        }
    }

    void OnApplicationQuit()
    {
        writer.Close();
    }

    public void NewSimulation()
    {
        infectionDeathRate += 0.2f;
        if (infectionDeathRate > 0.70f)
        {
            infectionDeathRate = 0.05f;
            infectionRate += 0.2f;
            if (infectionRate > 0.70f)
            {
                infectionRate = 0.05f;
                reInfectionRate += 0.2f;
                if (reInfectionRate > 0.70f)
                {
                    reInfectionRate = 0.05f;
                    recoveryRate += 0.2f;
                    if (recoveryRate > 0.70f)
                    {
                        recoveryRate = 0.05f;
                        symptomsRate += 0.2f;
                        if (symptomsRate > 0.70f)
                        {
                            Debug.Log("Koniec");
                            Application.Quit();
                        }
                    }
                }
            }
        }
        Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            Destroy(dots[i]);
        }
        stateEveryYear = new List<WorldState>();
        dotsStateEveryYear = new List<DotState[]>();

        transform.localScale = new Vector3(areaHeight, areaWidth, 1f);

        //Random.InitState((int)DateTime.Now.Ticks);

        float dotRadius = notInfected.transform.localScale.x / 2;
        for (int i = 0; i < numberOfDots; i++)
        {
            minX = transform.position.x - transform.localScale.x / 2 + dotRadius;
            maxX = transform.position.x + transform.localScale.x / 2 - dotRadius;
            minY = transform.position.y - transform.localScale.y / 2 + dotRadius;
            maxY = transform.position.y + transform.localScale.y / 2 - dotRadius;

            Vector3 randomSpawn = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
            dots[i] = Instantiate(notInfected, randomSpawn, randomRotation);
        }

        for (int i = 0; i < numberOfInfected; i++)
        {
            if (i >= numberOfDots)
                return;
            dots[i].gameObject.tag = "Infected";
        }
        currentYear = -1;
    }

    public float GetInfectionDeathRate() { return infectionDeathRate; }
    public float GetInfectionRate() { return infectionRate; }
    public float GetReInfectionRate() { return reInfectionRate; }
    public float GetRecoveryRate() { return recoveryRate; }
    public float GetSymptomsRate() { return symptomsRate; }


}