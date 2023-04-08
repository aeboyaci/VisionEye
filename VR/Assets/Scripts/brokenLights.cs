using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brokenLights : MonoBehaviour
{
    Light light;
    void Start()
    {
        light = GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {
        
        float currentState = Time.time%34;
        if (currentState < 7) { light.enabled = true; }
        else if(currentState<9.1){ light.enabled = false; }
        else if (currentState < 9.2) { light.enabled = true; }
        else if (currentState <9.3) { light.enabled = false; }
        else if (currentState < 13.1) { light.enabled = false; }
        else if (currentState < 13.4) { light.enabled = true; }
        else if (currentState < 13.5) { light.enabled = false; }
        else if (currentState < 17.1) { light.enabled = false; }
        else if (currentState < 17.3) { light.enabled = true; }
        else if (currentState < 17.5) { light.enabled = false; }
        else if (currentState < 22) { light.enabled = true; }
        else if (currentState < 25) { light.enabled = false; }
        else if (currentState < 27.1) { light.enabled = false; }
        else if (currentState < 27.2) { light.enabled = true; }
        else if (currentState < 27.3) { light.enabled = false; }
        else if (currentState < 33) { light.enabled = true; }
        //Start the coroutine we define below named ExampleCoroutine.
       // StartCoroutine(ExampleCoroutine());
    }
}
