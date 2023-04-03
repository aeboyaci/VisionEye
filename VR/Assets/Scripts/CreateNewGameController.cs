using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamPlayer
{
    public string DisplayName { get; set; }
    public string AvatarUrl { get; set; }
}

public class TeamInvitationResponse
{
    [JsonProperty("playerId")]
    public string playerId;

    [JsonProperty("displayName")]
    public string displayName;

    [JsonProperty("avatarUrl")]
    public string avatarUrl;

    [JsonProperty("status")]
    public string status;
}

public class RelayServerResponse
{
    [JsonProperty("ipv4")]
    public string ipv4;

    [JsonProperty("port")]
    public int port;

    [JsonProperty("joinCode")]
    public string joinCode;

    [JsonProperty("hasStarted")]
    public bool hasStarted;
}

public class CreateNewGameController : MonoBehaviour
{
    public TMPro.TMP_Text displayName;
    public TMPro.TMP_Text numberOfPlayersText;
    public Button randomButton;
    public TMPro.TMP_InputField teamNameInputField;
    public Image onlinePlayersGrid;
    public Image teamPlayersGrid;
    public Button nextButton;
    public Button goBackButton;

    public SendingInvitation sendingInvitationPrefab;
    public TeamPlayerCard teamPlayerCardPrefab;

    private HashSet<string> teamPlayerIds;

    private Dictionary<string, TeamPlayerCard> teamPlayerObjectMap;
    private Dictionary<string, SendingInvitation> onlinePlayerObjectMap;

    void Start()
    {
        displayName.text = State.DisplayName;

        randomButton.onClick.AddListener(randomButtonOnClick);
        goBackButton.onClick.AddListener(goBackButtonOnClick);
    }

    void OnEnable()
    {
        if (teamPlayerObjectMap == null || onlinePlayerObjectMap == null)
        {
            resetOrInitializeVariables();
        }

        foreach (string key in teamPlayerObjectMap.Keys)
        {
            Destroy(teamPlayerObjectMap[key].gameObject);
        }
        foreach (string key in onlinePlayerObjectMap.Keys)
        {
            Destroy(onlinePlayerObjectMap[key].gameObject);
        }
        resetOrInitializeVariables();

        numberOfPlayersText.text = "Players (1/4)";
        teamPlayerIds.Add(State.PlayerId);

        TeamPlayerCard card = Instantiate(teamPlayerCardPrefab, teamPlayersGrid.transform).GetComponent<TeamPlayerCard>();
        card.playerId = State.PlayerId;
        card.displayName.text = State.DisplayName;
        teamPlayerObjectMap[State.PlayerId] = card;

        if (!State.IsCaptain)
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            card.captainText.gameObject.SetActive(true);
        }

        StartCoroutine(PollRelayServerInformation_Coroutine());
        StartCoroutine(GetOnlinePlayers_Coroutine());
        StartCoroutine(GetTeamPlayers_Coroutine());
    }

    void OnDisable()
    {
        StopCoroutine(PollRelayServerInformation_Coroutine());
        StopCoroutine(GetOnlinePlayers_Coroutine());
        StopCoroutine(GetTeamPlayers_Coroutine());
    }

    private void resetOrInitializeVariables()
    {
        teamPlayerIds = new HashSet<string>();

        teamPlayerObjectMap = new Dictionary<string, TeamPlayerCard>();
        onlinePlayerObjectMap = new Dictionary<string, SendingInvitation>();

        teamNameInputField.text = "";
    }

    private void goBackButtonOnClick()
    {
        StartCoroutine(Delete_Coroutine());
    }

    private void goBackHome()
    {
        GameObject homeScreen = GameObject.Find("HomeScreen");
        gameObject.SetActive(false);
        homeScreen.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void randomButtonOnClick()
    {
        System.Random r = new System.Random();

        int length = 8;
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };

        string teamName = "";
        teamName += consonants[r.Next(consonants.Length)].ToUpper();
        teamName += vowels[r.Next(vowels.Length)];
        int b = 2;
        while (b < length)
        {
            teamName += consonants[r.Next(consonants.Length)];
            b++;
            teamName += vowels[r.Next(vowels.Length)];
            b++;
        }

        teamNameInputField.text = teamName;
    }

    IEnumerator Delete_Coroutine()
    {
        UnityWebRequest request;

        if (State.IsCaptain)
        {
            request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/delete");
            yield return request.SendWebRequest();
        }
        else
        {
            request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/delete/player/{State.PlayerId}");
            yield return request.SendWebRequest();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            goBackHome();
        }
    }

    IEnumerator PollRelayServerInformation_Coroutine()
    {
        while (true)
        {
            UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/relay-server");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Response response = Client.GetResponseValue(request);
                RelayServerResponse relayServer = JsonConvert.DeserializeObject<RelayServerResponse>(response.data.ToString());

                if (relayServer.hasStarted)
                {
                    // TODO: navigate to the map
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator GetOnlinePlayers_Coroutine()
    {
        while (true)
        {
            UnityWebRequest request = Client.PrepareRequest("GET", "/players");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Response response = Client.GetResponseValue(request);
                Player[] onlinePlayers = JsonConvert.DeserializeObject<Player[]>(response.data.ToString());

                for (int i = 0; i < onlinePlayers.Length; i++)
                {
                    Player player = onlinePlayers[i];
                    if (player.playerId.Equals(State.PlayerId) || onlinePlayerObjectMap.ContainsKey(player.playerId))
                    {
                        continue;
                    }

                    SendingInvitation invitation = Instantiate(sendingInvitationPrefab, onlinePlayersGrid.transform).GetComponent<SendingInvitation>();
                    invitation.playerId = player.playerId;
                    invitation.displayName.text = player.displayName;

                    onlinePlayerObjectMap[player.playerId] = invitation;
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator GetTeamPlayers_Coroutine()
    {
        while (true)
        {
            UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Response response = Client.GetResponseValue(request);
                TeamResponse teamResponse = JsonConvert.DeserializeObject<TeamResponse>(response.data.ToString());

                for (int i = 0; i < teamResponse.players.Count; i++)
                {
                    Player player = teamResponse.players[i];
                    if (player.playerId.Equals(State.PlayerId) || teamPlayerIds.Contains(player.playerId))
                    {
                        continue;
                    }

                    teamPlayerIds.Add(player.playerId);

                    if (teamPlayerIds.Count < 5)
                    {
                        TeamPlayerCard card = Instantiate(teamPlayerCardPrefab, teamPlayersGrid.transform).GetComponent<TeamPlayerCard>();
                        card.teamPlayerIds = teamPlayerIds;
                        card.onlinePlayersMap = onlinePlayerObjectMap;
                        card.numberOfPlayersText = numberOfPlayersText;

                        card.playerId = player.playerId;
                        card.displayName.text = player.displayName;
                        if (player.isCaptain)
                        {
                            card.captainText.gameObject.SetActive(true);
                        }

                        teamPlayerObjectMap[player.playerId] = card;

                        numberOfPlayersText.text = $"Players ({teamPlayerIds.Count}/4)";

                        Destroy(onlinePlayerObjectMap[player.playerId].gameObject);
                    }
                }
            }
            else if (request.result != UnityWebRequest.Result.InProgress && request.result != UnityWebRequest.Result.Success)
            {
                goBackButtonOnClick();
                break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
