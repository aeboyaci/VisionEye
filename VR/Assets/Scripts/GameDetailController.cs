using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

class GameDetailsResponse
{
    [JsonProperty("roomName")]
    public string roomName;

    [JsonProperty("minutesPlayed")]
    public int minutesPlayed;

    [JsonProperty("score")]
    public int score;

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
    public int score;

    [JsonProperty("owner")]
    public string owner;
}

public class GameDetailController : MonoBehaviour
{
    public TMPro.TMP_Text displayName;

    public TMPro.TMP_Text roomName;
    public TMPro.TMP_Text score;
    public TMPro.TMP_Text completeTime;

    public GameObject achievements;
    public AchievementRow achievementRowPrefab;

    void Start()
    {
        // TODO read from State
        displayName.text = "Ahmet Eren";

        StartCoroutine(GetGameDetailInformation_Coroutine());
    }
    
    IEnumerator GetGameDetailInformation_Coroutine()
    {
        // TODO read game id from State
        UnityWebRequest request = Client.PrepareRequest("GET", $"/games/5a399cc4-1699-41f9-bc43-3716f31842ca");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            GameDetailsResponse statusResponse = JsonConvert.DeserializeObject<GameDetailsResponse>(response.data.ToString());

            roomName.text = statusResponse.roomName;
            score.text = statusResponse.score.ToString();
            completeTime.text = statusResponse.minutesPlayed.ToString();

            foreach (AchievementData ach in statusResponse.achievements)
            {
                AchievementRow row = Instantiate(achievementRowPrefab, achievements.transform).GetComponent<AchievementRow>();
                row.name.text = ach.name;
                row.score.text = ach.score.ToString();
                row.owner.text = ach.owner;
                row.description.text = ach.description;
            }
        }
    }
}
