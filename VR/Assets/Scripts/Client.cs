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
    private static readonly string API_URL = "http://localhost:8080";

    private static string AccessToken { get; set; }
    private static string IdToken { get; set; }

    public static void SetTokens(string access, string id)
    {
        AccessToken = access;
        IdToken = id;
    }

    public static UnityWebRequest PrepareRequest(string method, string endpoint, object body = null)
    {
        UnityWebRequest request = new UnityWebRequest(API_URL + endpoint);
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

        if (AccessToken != null && IdToken != null)
        {
            request.SetRequestHeader("access_token", AccessToken);
            request.SetRequestHeader("id_token", IdToken);
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
