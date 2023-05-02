using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameEndBody
{
    [JsonProperty("teamId")]
    public string teamId;
}

public class PlayerPositionController : MonoBehaviour
{
    void Update()
    {
        if (transform.position.z > 2)
        {
            StartCoroutine(GameEnd_Coroutine());
            SceneManager.LoadScene("MainUI");
        }
    }

    IEnumerator GameEnd_Coroutine()
    {
        GameEndBody answer = new GameEndBody{ teamId = ScaryVerseState.ActiveTeamId };

        UnityWebRequest request = Client.PrepareRequest("POST", $"/games/{ScaryVerseState.ActiveGameId}", answer);
        yield return request.SendWebRequest();
    }
}
