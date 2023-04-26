using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class walkingSound : MonoBehaviour
{
    public AudioSource footstepSound;
    public GameObject player;
    float timer = 0f;
    Vector3 positionLast;
    public float footstepOption;
    private void Start()
    {
         positionLast = transform.position;
    }

    void Update()
    {
        Vector3 positionCurrent = transform.position;
        float distance = Vector3.Distance(positionCurrent, positionLast);
        //Debug.Log(distance);
        if (distance > footstepOption)
        {
            footstepSound.enabled= true;
            positionLast= positionCurrent;
            timer = Time.time;

        }
        else
        {
            float newtimer = Time.time;
            //Debug.Log(timer+ "   "+newtimer);
            if(newtimer>(timer+0.6))
            footstepSound.enabled= false;
        }


    }
}
