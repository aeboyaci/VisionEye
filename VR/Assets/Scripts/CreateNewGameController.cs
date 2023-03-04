using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateNewGameController : MonoBehaviour
{
    public TMPro.TMP_Text numberOfPlayersText;

    void Start()
    {
        StartCoroutine(GetTeamPlayers_Coroutine());
    }

    // TODO
    IEnumerator GetTeamPlayers_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", "/oauth2/google/passcode");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
        }
    }
}
