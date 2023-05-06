using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Area : MonoBehaviour
{
    public GameObject notInfected;
    public const int numberOfDots = 100;
    public float areaWidth = 5f;
    public float areaHeight = 5f;
    public int numberOfInfected;
    public GameObject[] dots = new GameObject[numberOfDots];
    //public float moveSpeed = 1f;

    float minX;
    float maxX;
    float minY;
    float maxY;

    void Start()
    {

        transform.localScale = new Vector3(areaHeight, areaWidth, 1f);

        Random.InitState(42);
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



    }
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.Space))
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

                dots[i].transform.position += dots[i].transform.up * dots[i].GetComponent<Dot>().moveSpeed * 7 * Time.deltaTime;

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
    }
}
