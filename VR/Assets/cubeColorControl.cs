using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeColorControl : MonoBehaviour
{

    public static string cubePassForDoor="";
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if(cubePassForDoor.Length>4)
        {
            cubePassForDoor = cubePassForDoor.Substring(cubePassForDoor.Length - 5);
        }
        if (cubePassForDoor == "ABCBA")
        {
            GetComponent<Renderer>().material.color = new Color(0, 204, 102);
            print("you can pass ");
        }
    }
}
