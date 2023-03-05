using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;


public  class ScoreBoardClass
{
    [JsonProperty("success")]
    public bool success;

    [JsonProperty("data")]
    public DataObj[] data;
}
public class DataObj
{
    [JsonProperty("playerId")]
    public string playerId;

    [JsonProperty("displayName")]
    public string displayName;

    [JsonProperty("avatarUrl")]
    public string avatarUrl;

    [JsonProperty("score")]
    public int score;
}
class DataObjComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return ((DataObj)x).score.CompareTo(((DataObj)y).score);
    }
}
public class ScoreBoard : MonoBehaviour
{

    public TMP_Text PlayerName;

    private TMPro.TMP_Text uiText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetCode_Coroutine());
       
    }

   

    IEnumerator GetCode_Coroutine()
    {
        while (true)
        {
            
    
        UnityWebRequest request = Client.PrepareRequest("GET", "/scoreboard");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            ScoreBoardClass statusResponse = JsonConvert.DeserializeObject<ScoreBoardClass>(response.data.ToString());

            PlayerName.text=statusResponse.data[0].displayName;


            Array.Sort( statusResponse.data, new DataObjComparer());

            Debug.Log(statusResponse.success);
            Debug.Log(statusResponse.data[0].playerId);
            Debug.Log(statusResponse.data[0].score);
            PlayerName.text = statusResponse.data[0].displayName;

            GameObject ScoreBoardsScreen = GameObject.Find("Grid");
            int i = 0;
            foreach (DataObj ach in statusResponse.data)
            {
                GameObject Satir = ScoreBoardsScreen.transform.GetChild(i).gameObject;
                    for(int y=1;y<3;y++)
                {
                    GameObject childOfChilds = Satir.transform.GetChild(y).gameObject;
                    uiText = childOfChilds.GetComponent<TextMeshProUGUI>();
                    if(y == 1)
                    {
                        uiText.text = "ach.displayName";
                    }
                    if (y == 2)
                    {
                        uiText.text = "ach.score.ToString()";
                    }

                }
                i++;
                if (i == 4)
                    break;
            }

            }
        
                yield return new WaitForSeconds(1.0f);
            }

            
        }
    }