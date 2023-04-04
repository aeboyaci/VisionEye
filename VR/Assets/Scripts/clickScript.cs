using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class clickScript : MonoBehaviour
{
    public TMPro.TMP_Text textOfButton;
    public GameObject button;
    private Animator mAnimator;
    void Start()
    {
        mAnimator= GetComponent<Animator>();
    }
   

    // Update is called once per frame
    void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Select stage    
                    if (hit.transform == button.transform)
                    {
                    if(mAnimator!= null)
                    {
                        print("inside");
                        mAnimator.SetBool("push", true);
                        

                    }

                    cubeColorControl.cubePassForDoor = cubeColorControl.cubePassForDoor + textOfButton.text;

                    
                    print(cubeColorControl.cubePassForDoor);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
               mAnimator.SetBool("push", false);
        }
        }


    }
