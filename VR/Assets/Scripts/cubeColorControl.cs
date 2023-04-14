using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class cubeColorControl : MonoBehaviour
{

    public static string cubePassForDoor="";
    [SerializeField] public TMPro.TMP_Text textOfChest;
    public string answer = "";
    public GameObject door;
    private void Start()
    {
        for(int i =0; i < 5; i++)
        {
            float randomNumber = Random.Range(0, 3);
            randomNumber = (int)randomNumber;
            if (randomNumber == 0)
            {
                answer = answer + "A";
            }
            if (randomNumber == 1)
            {
                answer = answer + "B";
            }
            if (randomNumber == 2)
            {
                answer = answer + "C";
            }
        }
        textOfChest.text = answer;
    }
    // Update is called once per frame
    void Update()
    {
        if(cubePassForDoor.Length>4)
        {
            cubePassForDoor = cubePassForDoor.Substring(cubePassForDoor.Length - 5);
        }
        if (cubePassForDoor == answer)
        {
            GetComponent<Renderer>().material.color = new Color(0, 204, 102);
            door.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }
}
