using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class LogoutManagerController : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    async void OnApplicationQuit()
    {
        string url = Client.API_URL + "/oauth2/google/logout";
        url += "?access_token=" + Client.AccessToken;
        url += "&id_token=" + Client.IdToken;

        using var client = new HttpClient();
        var response = await client.GetAsync(url);
    }
}
