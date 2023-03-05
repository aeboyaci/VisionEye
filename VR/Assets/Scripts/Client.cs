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
    private static string AccessToken = "ya29.a0AVvZVspZAn7mgqo7sTV86DaXEyd7dEge4cRUK1-NAb_hSa4gHoPTg0MbRPiTB0hPjYrKzncDMTkf_vISrqiF-dI9X-_6261wcwWrnrcdUA-WfFW1Bw0Jrgk93aIvQwJQOkQ05ZkC3m-EtxdYmDg7uRRB2UQlaCgYKATASARASFQGbdwaI2XheDVpT7XE0xPbI_oElFw0163";
    private static string IdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjI1NWNjYTZlYzI4MTA2MDJkODBiZWM4OWU0NTZjNDQ5NWQ3NDE4YmIiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIzNTIzNzYwNDY3NTktNDRmb2pqZ2djZjlrZXZ0ZGd2Y284ZXNlOTI5ZGVjODkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIzNTIzNzYwNDY3NTktNDRmb2pqZ2djZjlrZXZ0ZGd2Y284ZXNlOTI5ZGVjODkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTcwOTkwOTg4MzIzNTE0Njg0NjgiLCJlbWFpbCI6InNjYXJ5dmVyc2UudmlzaW9uZXllQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdF9oYXNoIjoiNW1aNHF6anlkZEVGZzlkYVhIRUFiZyIsIm5hbWUiOiJWaXNpb24gRXllIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FHTm15eGF2dGRCSWxoYm9ESUJ4Sl92LWI4ZGJlakpoVVRyS1N4eThOYlVCPXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IlZpc2lvbiIsImZhbWlseV9uYW1lIjoiRXllIiwibG9jYWxlIjoiZW4iLCJpYXQiOjE2NzgwMTY1MDMsImV4cCI6MTY3ODAyMDEwM30.X9-8S-4_wrDRrvnk7wwdfHJjHeTa1eEPUMkCRo2AN8eh3LGXCmHp_mn3xp7uvG00Pfw5-rz9TpofhN2CJ6cQqF18znCiBQ5nhp9yHdRYHVHblsSRvB4O4cQGJTg-JIweKDoop4GIccZdzSMIthMEE-NLfAY3MciibWeg18dcYvoK7QbPSh0IqhJQ8K3LJPC-HiZxEeqXDnRHr8sOvCJhs6MSZjj1gOHFUPso-yp2RbvadDG7RQa2cu5LaxjlknYxeA3ne7TyEooB-3rD_rk-mIUY_6uB5-JG-CrEq0YoBf4QRC9msd6v04q-knnZeW9wG0ZtJqJu9_MHs3i16sEgrA";


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
