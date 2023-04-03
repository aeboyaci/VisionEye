using System.Collections;
using System.Collections.Generic;
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

    private Dictionary<string, AchievementRow> achievementObjectMap;

    void Start()
    {
        displayName.text = State.DisplayName;
    }

    void OnEnable()
    {
        if (achievementObjectMap == null)
        {
            achievementObjectMap = new Dictionary<string, AchievementRow>();
        }

        foreach (string key in achievementObjectMap.Keys)
        {
            Destroy(achievementObjectMap[key].gameObject);
        }
        achievementObjectMap = new Dictionary<string, AchievementRow>();

        StartCoroutine(GetGameDetailInformation_Coroutine());
    }

    void OnDisable()
    {
        StopCoroutine(GetGameDetailInformation_Coroutine());
    }

    IEnumerator GetGameDetailInformation_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", $"/games/{State.ActiveGameId}");
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

                achievementObjectMap[ach.id] = row;
            }
        }
    }
}
