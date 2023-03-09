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
    private static string AccessToken = "ya29.a0AVvZVsod14RM8-NpKKT7YP9SdchSXQ5uLlZf3-_Zlx6_R2p6aLhodl9y5C8hNH96NzrUS53XLaWJuLABFpE6Z4axKfEsFwkTS_WFIK5mcm2LcLW6sjWtt70f_Csew7Y9YwAMB9vgwAGa23tMEc_EDlAr2AgzaCgYKAb4SARASFQGbdwaIpKD9KBPMdbkpUvDNqEjQkw0163";
    private static string IdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjI1NWNjYTZlYzI4MTA2MDJkODBiZWM4OWU0NTZjNDQ5NWQ3NDE4YmIiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIzNTIzNzYwNDY3NTktNDRmb2pqZ2djZjlrZXZ0ZGd2Y284ZXNlOTI5ZGVjODkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIzNTIzNzYwNDY3NTktNDRmb2pqZ2djZjlrZXZ0ZGd2Y284ZXNlOTI5ZGVjODkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTcwOTkwOTg4MzIzNTE0Njg0NjgiLCJlbWFpbCI6InNjYXJ5dmVyc2UudmlzaW9uZXllQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdF9oYXNoIjoiSVdHaGx5RjY4MkRIN0NNc0N2SXBoZyIsIm5hbWUiOiJWaXNpb24gRXllIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FHTm15eGF2dGRCSWxoYm9ESUJ4Sl92LWI4ZGJlakpoVVRyS1N4eThOYlVCPXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IlZpc2lvbiIsImZhbWlseV9uYW1lIjoiRXllIiwibG9jYWxlIjoiZW4iLCJpYXQiOjE2NzgxOTg2MTQsImV4cCI6MTY3ODIwMjIxNH0.BL82vCgzVJBSeTFqTpAK-IImhqu_aPz0neIyfEn0UBfM8oKKpgjfPAGxVYg6xze_3BWWG0OJphDgY4wC_sNvwtYVx70Wldk0a8d_X3R6HgBFnUj2exoBkBSNSYeQZMUgQoj0DrOaHWlB130Lj1AsiqNhQIzfmT-CWSeOYyFTWDlXRAAmwwIfrqu29ZD59Lrr84kxudmhBsWyas3Ca73J14zZOmwT3Oil_A3Rasqlmy8-y0j-rsXoRceMjl6kBOzivjd0uX4BXYqGwpWxkoG2hWdOF4kZgRNZsQ0ReyePW_U0ceg-SStQb78G1_qMaXbB7lMdzDYEhRJgQPIgsk7TSg";

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
            Debug.Log(url);
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
