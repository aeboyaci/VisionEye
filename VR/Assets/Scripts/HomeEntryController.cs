using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class PlayerInformationResponse
{
    [JsonProperty("displayName")]
    public string displayName;

    [JsonProperty("avatarUrl")]
    public string avatarUrl;
}

public class HomeEntryController : MonoBehaviour
{
    public TMPro.TMP_Text playerDisplayName;

    void Start()
    {
        StartCoroutine(GetPlayerInformation_Coroutine());
    }

    IEnumerator GetPlayerInformation_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", "/players/me");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            PlayerInformationResponse playerInformation = JsonConvert.DeserializeObject<PlayerInformationResponse>(response.data.ToString());

            playerDisplayName.text = playerInformation.displayName;
            playerDisplayName.ForceMeshUpdate(true);
        }
    }
}
