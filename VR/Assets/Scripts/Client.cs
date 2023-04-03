using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class Response
{
    [JsonProperty("success")]
    public bool success;

    [JsonProperty("data")]
    public object data;
}

public class Client
{
    public static readonly string API_URL = "https://scary-verse.com";

    public static string AccessToken { get; set; }
    public static string IdToken { get; set; }

    public static UnityWebRequest PrepareRequest(string method, string endpoint, object body = null)
    {
        string url = API_URL + endpoint;
        if (AccessToken != null && IdToken != null)
        {
            url += "?access_token=" + AccessToken;
            url += "&id_token=" + IdToken;
        }

        UnityWebRequest request = new UnityWebRequest(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.method = method;

        if (method.Equals("POST"))
        {
            if (body == null)
            {
                throw new Exception("HTTP body cannot be NULL in POST requests");
            }

            request.SetRequestHeader("Content-Type", "application/json");
            string json = JsonConvert.SerializeObject(body);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        }

        return request;
    }

    public static Response GetResponseValue(UnityWebRequest request)
    {
        string json = request.downloadHandler.text;
        Response response = JsonConvert.DeserializeObject<Response>(json);
        return response;
    }
}
