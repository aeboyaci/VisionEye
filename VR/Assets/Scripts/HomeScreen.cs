using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


class PastTeamResponse {

    [JsonProperty("id")]
    public string id;
    [JsonProperty("name")]
    public string name;
    [JsonProperty("totalScoreGained")]
    public string totalScoreGained;
    [JsonProperty("minutesPlayed")]
    public string minutesPlayed;

}

class Me {
    [JsonProperty("id")]
    public string id;
    [JsonProperty("displayName")]
    public string displayName;
    [JsonProperty("avatarUrl")]
    public string avatarUrl;
}
class InvitationResponse {


    [JsonProperty("teamName")]
    public string teamName;
    [JsonProperty("sender")]
    public string sender;
}

class Player {
    [JsonProperty("playerId")]
    public string playerId;
    [JsonProperty("displayName")]
    public string displayName;
    [JsonProperty("avatarUrl")]
    public string avatarUrl;
    [JsonProperty("isCaptain")]
    public bool isCaptain;
    [JsonProperty("isOnline")]
    public bool isOnline;
}

class Sender {

    [JsonProperty("playerId")]
    public string playerId;
    [JsonProperty("displayName")]
    public string displayName;
    [JsonProperty("avatarUrl")]
    public string avatarUrl;

}
class CreateTeamResponse
{
    [JsonProperty("success")]
    public bool success;
    [JsonProperty("teamId")]
    public string teamId;
}

class AnswerInvitation {
    [JsonProperty("id")]
    public string id;
    [JsonProperty("status")]
    public string status;

}

public class HomeScreen : MonoBehaviour
{
    public PastTeam pastTeam;
    public Invitation invitationObj;

    public TMPro.TMP_Text screenDisplayName;
    public Button createNewGameButton;
    public GameObject teamContainer;
    public GameObject invitationContainer;

    private void OnCreateNewGameButtonClick()
    {
        StartCoroutine(CreateNewGame_Coroutine());
    }

    IEnumerator GetPastTeams_Coroutine()
    {
      
        UnityWebRequest request = Client.PrepareRequest("GET", "/teams");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            
            Response response = Client.GetResponseValue(request);
            List<PastTeamResponse> teams = JsonConvert.DeserializeObject<List<PastTeamResponse>>(response.data.ToString());
            Debug.Log(teams.Count);
            for (int i = 0; i < teams.Count; i++)
            {
                PastTeamResponse team = teams[i];
                if (pastTeam != null)
                {

                    var row = Instantiate(pastTeam, teamContainer.transform).GetComponent<PastTeam>();
                    row.name.text = team.name;
                    row.totalScoreGained.text = team.totalScoreGained;
                    row.minutesPlayed.text = team.minutesPlayed;
                    row.id = team.id;

                }
            }
        }
    }

    IEnumerator GetMyInfo_Coroutine()
    {

        UnityWebRequest request = Client.PrepareRequest("GET", "/players/me");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            Me myself = JsonConvert.DeserializeObject<Me>(response.data.ToString());
            State.PlayerId = myself.id;
            State.DisplayName = myself.displayName;
        }
        else Debug.Log(request.result);

    }

    IEnumerator CreateNewGame_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", "/teams/create");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            CreateTeamResponse createTeamResponse = JsonConvert.DeserializeObject<CreateTeamResponse>(response.data.ToString());

            if (createTeamResponse.success == true)
            {
                State.ActiveTeamId = createTeamResponse.teamId;

                GameObject createNewGameScreen = GameObject.Find("CreateNewGameScreen");
                gameObject.SetActive(false);
                createNewGameScreen.transform.GetChild(0).gameObject.SetActive(true);
            }
            else {
                throw new Exception("New Game could not be created!");
            }
        }

    }

    IEnumerator GetInvitations_Coroutine() {
        while (true)
        {
            UnityWebRequest request = Client.PrepareRequest("GET", "/invitations");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Response response = Client.GetResponseValue(request);
                List<InvitationResponse> invitations = JsonConvert.DeserializeObject<List<InvitationResponse>>(response.data.ToString());

                for (int i = 0; i < invitations.Count; i++)
                {
                    InvitationResponse invitation = invitations[i];
                    if (invitationObj != null)
                    {
                        var row = Instantiate(invitationObj, invitationContainer.transform).GetComponent<Invitation>();
                        row.teamName.text = invitation.teamName;
                        row.sender.text = invitation.sender;

                    }
                }
            }
            else Debug.Log(request.result);

            yield return new WaitForSeconds(1.0f);
        }

    }

    void Start()
    {
        createNewGameButton.onClick.AddListener(OnCreateNewGameButtonClick);
        StartCoroutine(GetMyInfo_Coroutine());
        StartCoroutine(GetPastTeams_Coroutine());
        StartCoroutine(GetInvitations_Coroutine());
    }
}
