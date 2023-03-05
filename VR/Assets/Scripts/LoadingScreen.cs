using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{

    public TMPro.TMP_Text uiText;

    float b;
    float a;
    // Start is called before the first frame update
    void Start()
    {
        b = 0.03f;
         a = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        System.Random random = new System.Random();
       float x = random.Next(10);
        a = (float)(a + b);

        if (a >= 20)
        {
            b = b * 1.001f;

        }
        if (a >= 50)
        {
            b = b * 1.000002f;

        }
        if (a >= 100)
        {
            a = 100;
            GameObject LoadingScreen = GameObject.Find("LoadingScreen");
            gameObject.SetActive(false);

        }
        uiText.text = "%" + (int)a;
        
    }
}
