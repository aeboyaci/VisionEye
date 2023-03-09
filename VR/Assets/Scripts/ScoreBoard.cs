using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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


public class ScoreBoard : MonoBehaviour
{

    public TMP_Text playerDisplayName;
    public GameObject scoreBoardObject;
    public PrefabClassForScoreBoard scoreboardRowPrefab;
    // Start is called before the first frame update
    void Start()
    {
        playerDisplayName.text = State.DisplayName;

        StartCoroutine(GetCode_Coroutine());
        
    }
    IEnumerator GetCode_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", "/scoreboard");
        yield return request.SendWebRequest();
         if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            DataObj[] statusResponse = JsonConvert.DeserializeObject<DataObj[]>(response.data.ToString());

            int i = 0;
            foreach (DataObj ach in statusResponse)
            {
                if (i == 5)
                    break;
                i++;
                PrefabClassForScoreBoard row = Instantiate(scoreboardRowPrefab, scoreBoardObject.transform).GetComponent<PrefabClassForScoreBoard>();
                row.name_Player.text = ach.displayName;
                row.score.text = ach.score.ToString();
                row.Rank.text=i.ToString()+".";
 
            }

                
        }
        }
    }
