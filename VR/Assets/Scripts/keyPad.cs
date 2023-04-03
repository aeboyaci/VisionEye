using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyPad : MonoBehaviour
{
    [SerializeField] public TMPro.TMP_Text Ans;
    public string answer = "1234";
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
        }
        else
        {
            Ans.text="Invalid";
        }
    }
}
