using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionZone : MonoBehaviour
{
    public GameObject connectedDot;
    private Dot dotScript;
    public float DeltaTime = 0;

    private void Start()
    {
        dotScript = connectedDot.GetComponent<Dot>();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        DeltaTime += Time.deltaTime;
        if(DeltaTime >= dotScript.GetActionLength())
        {
            DeltaTime = 0;
            if (gameObject.tag != "Dead")
            {
                if (gameObject.tag == "Infected" || gameObject.tag == "SInfected")
                {
                    if (other.gameObject.tag == "Uninfected" && (float)Random.Range(0, 100000) / 100000 <= dotScript.infectionProb[dotScript.GetLiveTime()] || other.gameObject.tag == "Recovered" && (float)Random.Range(0, 100000) / 100000 <= dotScript.reInfectionProb[dotScript.GetLiveTime()])
                    {
                        other.gameObject.tag = "Infected";
                    }
                }
            }
        }
    }
}
