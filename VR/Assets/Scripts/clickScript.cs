using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;


public class clickScript : MonoBehaviour
{
    public TMPro.TMP_Text textOfButton;
    public GameObject button;
    private Animator mAnimator;

    public Button buttonToPush;
    void Start()
    {
        mAnimator= GetComponent<Animator>();
        buttonToPush.onClick.AddListener(onButtonClick);
    }

    private void onButtonClick()
    {
        if (mAnimator != null)
        {
            mAnimator.SetBool("push", true);


        }
        Invoke("WaitAndDoSomething", 2f);


        cubeColorControl.cubePassForDoor = cubeColorControl.cubePassForDoor + textOfButton.text;
        Debug.Log(cubeColorControl.cubePassForDoor);
    }

    void WaitAndDoSomething()
    {
        mAnimator.SetBool("push", false);
    }
    // Update is called once per frame



    }
