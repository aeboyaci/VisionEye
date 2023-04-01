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
    private Dictionary<string, SendingInvitation> onlinePlayersMap;

    /*public async void CreateRelay()
    {
        try
        {

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code is " + " " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            await File.WriteAllTextAsync("joinCode.txt", joinCode);
            Debug.Log("The join code has been saved.");
            //System.Threading.Thread.Sleep(1000*20);


            SceneManager.LoadScene("NewScene");

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void JoinRelay()
    {
        try
        {
            string joinCode = File.ReadAllText("joinCode.txt");
            Debug.Log("Joining relay with " + " " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();

            SceneManager.LoadScene("NewScene");


        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }


    }

    */
    void Start()
    {
        teamPlayerIds = new HashSet<string>();
        onlinePlayersMap = new Dictionary<string, SendingInvitation>();

        displayName.text = State.DisplayName;

        numberOfPlayersText.text = "Players (1/4)";
        teamPlayerIds.Add(State.PlayerId);

        TeamPlayerCard card = Instantiate(teamPlayerCardPrefab, teamPlayersGrid.transform).GetComponent<TeamPlayerCard>();
        card.displayName.text = State.DisplayName;

        if (!State.IsCaptain)
        {
            nextButton.gameObject.SetActive(false);
        }

        randomButton.onClick.AddListener(randomButtonOnClick);
        goBackButton.onClick.AddListener(goBackButtonOnClick);

        StartCoroutine(PollRelayServerInformation_Coroutine());
        StartCoroutine(GetOnlinePlayers_Coroutine());
        StartCoroutine(GetTeamPlayers_Coroutine());
    }

    private void goBackButtonOnClick()
    {
        if (State.IsCaptain)
        {
            StartCoroutine(DeleteTeam_Coroutine());
        }
        else
        {
            goBackHome();
        }
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

    IEnumerator DeleteTeam_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/delete");
        yield return request.SendWebRequest();

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
                    if (player.playerId.Equals(State.PlayerId) || onlinePlayersMap.ContainsKey(player.playerId))
                    {
                        continue;
                    }

                    SendingInvitation invitation = Instantiate(sendingInvitationPrefab, onlinePlayersGrid.transform).GetComponent<SendingInvitation>();
                    invitation.playerId = player.playerId;
                    invitation.displayName.text = player.displayName;

                    onlinePlayersMap[player.playerId] = invitation;
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
                    if (player.playerId.Equals(State.PlayerId))
                    {
                        continue;
                    }

                    teamPlayerIds.Add(player.playerId);

                    if (teamPlayerIds.Count < 5)
                    {
                        TeamPlayerCard card = Instantiate(teamPlayerCardPrefab, teamPlayersGrid.transform).GetComponent<TeamPlayerCard>();
                        card.displayName.text = player.displayName;
                        if (player.isCaptain)
                        {
                            card.captainText.gameObject.SetActive(true);
                        }

                        numberOfPlayersText.text = $"Players ({teamPlayerIds.Count}/4)";

                        Destroy(onlinePlayersMap[player.playerId].gameObject);
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
