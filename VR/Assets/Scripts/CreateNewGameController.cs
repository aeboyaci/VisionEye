using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

    public SendingInvitation sendingInvitationPrefab;
    public TeamPlayerCard teamPlayerCardPrefab;

    private List<TeamPlayer> teamPlayers;

    private HashSet<string> teamPlayerIds;
    private HashSet<string> onlinePlayerIds;

    void Start()
    {
        teamPlayers = new List<TeamPlayer>();

        teamPlayerIds = new HashSet<string>();
        onlinePlayerIds = new HashSet<string>();

        displayName.text = State.DisplayName;

        numberOfPlayersText.text = "Players (1/4)";
        TeamPlayer me = new TeamPlayer { DisplayName = State.DisplayName, AvatarUrl = State.AvatarUrl };
        teamPlayers.Add(me);
        TeamPlayerCard card = Instantiate(teamPlayerCardPrefab, teamPlayersGrid.transform).GetComponent<TeamPlayerCard>();
        card.displayName.text = me.DisplayName;

        if (!State.IsCaptain)
        {
            nextButton.gameObject.SetActive(false);
        }

        randomButton.onClick.AddListener(randomButtonOnClick);

        StartCoroutine(PollRelayServerInformation_Coroutine());
        StartCoroutine(GetOnlinePlayers_Coroutine());
        StartCoroutine(GetTeamPlayers_Coroutine());
    }

    private void goBackButtonOnClick()
    {
        StartCoroutine(DeleteTeam_Coroutine());
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

    IEnumerator DeleteTeam_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/delete");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // TODO go back to Home Screen
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
                    // TODO connect to the captain's relay-server
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
                    if (player.playerId.Equals(State.PlayerId) || onlinePlayerIds.Contains(player.playerId))
                    {
                        continue;
                    }

                    onlinePlayerIds.Add(player.playerId);

                    SendingInvitation invitation = Instantiate(sendingInvitationPrefab, onlinePlayersGrid.transform).GetComponent<SendingInvitation>();
                    invitation.playerId = player.playerId;
                    invitation.displayName.text = player.displayName;
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator GetTeamPlayers_Coroutine()
    {
        while (true)
        {
            UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/invitations");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Response response = Client.GetResponseValue(request);
                TeamInvitationResponse[] teamInvitationResponse = JsonConvert.DeserializeObject<TeamInvitationResponse[]>(response.data.ToString());

                for (int i = 0; i < teamInvitationResponse.Length; i++)
                {
                    TeamInvitationResponse invitation = teamInvitationResponse[i];
                    if (invitation.playerId.Equals(State.PlayerId))
                    {
                        continue;
                    }

                    if (invitation.status.Equals("ACCEPTED") && !teamPlayerIds.Contains(invitation.playerId))
                    {
                        teamPlayerIds.Add(invitation.playerId);

                        if (teamPlayers.Count < 5)
                        {
                            TeamPlayer teamPlayer = new TeamPlayer { DisplayName = invitation.displayName, AvatarUrl = invitation.avatarUrl };
                            teamPlayers.Add(teamPlayer);
                            TeamPlayerCard card = Instantiate(teamPlayerCardPrefab, teamPlayersGrid.transform).GetComponent<TeamPlayerCard>();
                            card.displayName.text = teamPlayer.DisplayName;

                            numberOfPlayersText.text = $"Players ({teamPlayers.Count}/4)";
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
