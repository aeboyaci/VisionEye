using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

class GameDetailsResponse
{
    [JsonProperty("roomName")]
    public string roomName;
    [JsonProperty("minutesPlayed")]
    public string minutesPlayed;
    [JsonProperty("score")]
    public string score;
    [JsonProperty("achievements")]
    public AchievementData[] achievements;
}
class AchievementData
{
    [JsonProperty("id")]
    public string id;
    [JsonProperty("name")]
    public string name;
    [JsonProperty("description")]
    public string description;
    [JsonProperty("score")]
    public string score;
    [JsonProperty("owner")]
    public string owner;

}






public class GameDetailController : MonoBehaviour
{
    private TMPro.TMP_Text uiText;

    public TMPro.TMP_Text roomName;
    public TMPro.TMP_Text score;
    public TMPro.TMP_Text completeTime;



    private string gameId = null;
    private TextMeshPro UIText;

    void Start()
    {
     //    StartCoroutine(Poll_Coroutine());
    }
    

    
    IEnumerator Poll_Coroutine()
    {
        while (true)
        {
            
            if (gameId != null)
            {
                UnityWebRequest request = Client.PrepareRequest("GET", $"/games/{gameId}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Response response = Client.GetResponseValue(request);
                    GameDetailsResponse statusResponse = JsonConvert.DeserializeObject<GameDetailsResponse>(response.data.ToString());


                    roomName.text = statusResponse.roomName;
                    score.text = statusResponse.score;
                    completeTime.text = statusResponse.minutesPlayed; 


                    GameObject achievementsScreen = GameObject.Find("Achievements");
                    int i = 0;
                    foreach (AchievementData ach in statusResponse.achievements)
                    {
                        GameObject achievemts = achievementsScreen.transform.GetChild(i).gameObject;//ach1-2-3-4

                        for(int y=0;y<4;y++)
                        {
                            GameObject childOfChilds = achievemts.transform.GetChild(y).gameObject;
                            uiText = childOfChilds.GetComponent<TextMeshProUGUI>();
                            if(y == 0)
                            {
                                uiText.text = ach.name;
                            }
                            if (y == 1)
                            {
                                uiText.text = ach.score;
                            }
                            if (y == 2)
                            {
                                uiText.text = ach.owner;
                            }
                            if (y == 3)
                            {
                                uiText.text = ach.description;
                            }
                            uiText.text = ach.id;
                        }

                        i++;
                        if (i == 4)
                            break;

                    }

                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

