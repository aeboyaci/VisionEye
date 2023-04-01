using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyPad : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TMPro.TMP_Text Ans;
    public string answer = "3131";
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
            Ans.text="Helal mk";
        }
        else
        {
            Ans.text="Invalid";
        }
    }
}
