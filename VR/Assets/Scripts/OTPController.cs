using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

class StatusResponse
{
    [JsonProperty("status")]
    public string status;

    [JsonProperty("tokens")]
    public TokenData tokens;
}

class TokenData
{
    [JsonProperty("accessToken")]
    public string accessToken;

    [JsonProperty("idToken")]
    public string idToken;
}

public class OTPController : MonoBehaviour
{
    public TMPro.TMP_Text uiText;

    private string code = null;
    
    void Start()
    {
        StartCoroutine(GetCode_Coroutine());
        StartCoroutine(Poll_Coroutine());
    }

    IEnumerator GetCode_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", "/oauth2/google/passcode");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);

            code = (string)response.data;
            uiText.text = code.Substring(0, 3) + " " + code.Substring(3, 3);
            uiText.ForceMeshUpdate(true);
        }
    }

    IEnumerator Poll_Coroutine()
    {
        while (true)
        {
            if (code != null)
            {
                UnityWebRequest request = Client.PrepareRequest("GET", $"/oauth2/google/passcode/{code}/status");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Response response = Client.GetResponseValue(request);
                    StatusResponse statusResponse = JsonConvert.DeserializeObject<StatusResponse>(response.data.ToString());

                    string status = statusResponse.status;
                    if (status.Equals("COMPLETED"))
                    {
                        Client.SetTokens(statusResponse.tokens.accessToken, statusResponse.tokens.idToken);

                        Debug.Log(statusResponse.status);
                        Debug.Log(statusResponse.tokens.accessToken);
                        Debug.Log(statusResponse.tokens.idToken);

                        GameObject homeScreen = GameObject.Find("HomeScreen");
                        gameObject.SetActive(false);
                        homeScreen.transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
