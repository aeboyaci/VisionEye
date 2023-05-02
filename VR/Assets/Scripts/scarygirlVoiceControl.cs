using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class scarygirlVoiceControl : MonoBehaviour
{
    public AudioSource girlsound;
    public AudioSource voice1;
    public AudioSource voice2;
    public AudioSource voice3;

    private int idx = 0;


    public Button girlButton;
    void Start()
    {
        girlButton.onClick.AddListener(onButtonClick);
    }
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
    private void onButtonClick()
    {

        girlsound.enabled = false;
        voice1.enabled = false;
        voice2.enabled = false;
        voice3.enabled = false;

        if(idx == 0)
        {
            voice1.enabled = true;
        }
        if (idx == 1)
        {
            voice2.enabled = true;
        }
        if (idx == 2)
        {
            voice3.enabled = true;
        }
        idx = (idx + 1) % 3;
    }
}
