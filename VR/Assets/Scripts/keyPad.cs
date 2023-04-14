using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class keyPad : MonoBehaviour
{
    [SerializeField] public TMPro.TMP_Text Ans;
    public string answer = "";
    [SerializeField] public TMPro.TMP_Text puzzlesText;
    public GameObject door;


    private void Start()
    {
        float randomNumber = Random.Range(1000, 9999);
        randomNumber = (int)randomNumber;
        answer = randomNumber.ToString();
        puzzlesText.text = answer;
    }
    public void Number(int number)
    {   
        if(Ans.text=="Invalid"){
            Ans.text = number.ToString();
        }else{
            if(Ans.text.Length<4){
                Debug.Log(number);
                Ans.text += number.ToString();

            }
        }
    }

    public void execPass(){
        if(Ans.text == answer){
            Ans.text="Correct!";
            door.GetComponent<XRGrabInteractable>().enabled =true;   
        }
        else
        {
            Ans.text="Invalid";
        }
    }
}
