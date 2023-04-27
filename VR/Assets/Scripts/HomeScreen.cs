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


    [JsonProperty("name")]
    public string name;
    [JsonProperty("sender")]
    public Sender sender;
    [JsonProperty("id")]
    public string id;
    [JsonProperty("teamId")]
    public string teamId;

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
    
    private HashSet<string> invitationSet;
    private Dictionary<string, PastTeam> teamObjectMap;
    private Dictionary<string, Invitation> invitationObjectMap;

    void Start()
    {
        createNewGameButton.onClick.AddListener(OnCreateNewGameButtonClick);
    }

    void OnEnable()
    {
        if(teamObjectMap == null || invitationObjectMap == null)
        {
            resetOrInitializeVariables();
        }

        foreach (string key in teamObjectMap.Keys)
        {
            Destroy(teamObjectMap[key].gameObject);
        }
        foreach (string key in invitationObjectMap.Keys)
        {
            Destroy(invitationObjectMap[key].gameObject);
        }

        resetOrInitializeVariables();

        StartCoroutine(GetMyInfo_Coroutine());
        StartCoroutine(GetPastTeams_Coroutine());
        StartCoroutine(GetInvitations_Coroutine());
    }

    void OnDisable()
    {
        StopCoroutine(GetMyInfo_Coroutine());
        StopCoroutine(GetPastTeams_Coroutine());
        StopCoroutine(GetInvitations_Coroutine());        
    }

    private void resetOrInitializeVariables()
    {
        invitationSet = new HashSet<string>();
        teamObjectMap = new Dictionary<string, PastTeam>();
        invitationObjectMap = new Dictionary<string, Invitation>();
    }

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

                    teamObjectMap[team.id] = row;
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
            ScaryVerseState.PlayerId = myself.id;
            ScaryVerseState.DisplayName = myself.displayName;
            screenDisplayName.text = ScaryVerseState.DisplayName;
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

            ScaryVerseState.ActiveTeamId = createTeamResponse.teamId;
            ScaryVerseState.IsCaptain = true;

            GameObject createNewGameScreen = GameObject.Find("CreateNewGameScreen");
            gameObject.SetActive(false);
            createNewGameScreen.transform.GetChild(0).gameObject.SetActive(true);
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
                    if (invitationObj != null && !(invitationSet.Contains(invitation.sender.playerId)))
                    {
                        var row = Instantiate(invitationObj, invitationContainer.transform).GetComponent<Invitation>();
                        row.sender.text = invitation.sender.displayName;
                        row.id = invitation.id;
                        row.teamId = invitation.teamId;

                        invitationSet.Add(invitation.sender.playerId);
                        invitationObjectMap[invitation.id] = row;
                    }
                }
            }
            else Debug.Log(request.result);

            yield return new WaitForSeconds(1.0f);
        }
    }
}
