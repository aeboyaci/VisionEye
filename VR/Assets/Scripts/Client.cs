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
    private static readonly string API_URL = "https://scary-verse.com";

    private static string AccessToken = "ya29.a0AVvZVsrbfCt1IqnTV5OBgUf99X3g37UHsnd7GkROV09tm7wv1EAT731bzh6rJgYy_uHaeVf6wvNZElc4fE3Ug2vqVJdR246YWVISjRJ15QhwMeRjI2WDuSZTN0b9eZR1PP-Q9_GLENB_2KvKjcyLyu8u1IxgaCgYKAVMSARASFQGbdwaIBvMSbSYoB_ZWr7p12wkZ8w0163";
    private static string IdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjI1NWNjYTZlYzI4MTA2MDJkODBiZWM4OWU0NTZjNDQ5NWQ3NDE4YmIiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIzNTIzNzYwNDY3NTktNDRmb2pqZ2djZjlrZXZ0ZGd2Y284ZXNlOTI5ZGVjODkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIzNTIzNzYwNDY3NTktNDRmb2pqZ2djZjlrZXZ0ZGd2Y284ZXNlOTI5ZGVjODkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTcwOTkwOTg4MzIzNTE0Njg0NjgiLCJlbWFpbCI6InNjYXJ5dmVyc2UudmlzaW9uZXllQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdF9oYXNoIjoibzVfXzJZa2lObTJySGNldjZTRnZkdyIsIm5hbWUiOiJWaXNpb24gRXllIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FHTm15eGF2dGRCSWxoYm9ESUJ4Sl92LWI4ZGJlakpoVVRyS1N4eThOYlVCPXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IlZpc2lvbiIsImZhbWlseV9uYW1lIjoiRXllIiwibG9jYWxlIjoiZW4iLCJpYXQiOjE2NzgxMjUzMDEsImV4cCI6MTY3ODEyODkwMX0.Spc_LDbfpBIniAXWTYMdJiogsHJ2eW-dyYbFD2oxGcZBK22eFdBswhM_fxz8IWEb5XTO13XyJ1H6dhVS4uQemDrC39uSFJNuO1ySGa8Ew86IUds7TYHzxM_XWnxSZTP43_UvNRWp-91aadDCtC_bduwM94pHqCMncl-GIshbYEkRTBchsj-UBb6E_6XBOqmLKgDdJRaXNBnS4S_t0M5UjEI-e--KiUFe5tihr1c9vOlugzXdZV_-PEmmbDyUlkLP0WjYTyjLPxSUKjrGuutASCa3NTRUXBlpRenamSxUD1LJpOWZ8I1pfoy7hCtULY868natDnZxdPuqotsjjMrNFw";

    public static void SetTokens(string access, string id)
    {
        AccessToken = access;
        IdToken = id;
    }

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