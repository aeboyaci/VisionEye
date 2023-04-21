using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class scarygirlVoiceControl : MonoBehaviour
{
    public AudioSource girlsound;
 
    void Update()
    {
        

        if ((Time.time % 120) < 21 && Time.time > 20)
        {
            girlsound.enabled = true;
        }
        else
        {
            girlsound.enabled = false;
            
        }
        
    }
}
