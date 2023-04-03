using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

class Game
{
    [JsonProperty("roomName")]
    public string roomName;

    [JsonProperty("score")]
    public int? score;

    [JsonProperty("minutesPlayed")]
    public int? minutesPlayed;

    [JsonProperty("startedAt")]
    public string startedAt;

    [JsonProperty("id")]
    public string id;
}

class TeamResponse
{
    [JsonProperty("id")]
    public string id;

    [JsonProperty("name")]
    public string name;

    [JsonProperty("games")]
    public List<Game> games;

    [JsonProperty("players")]
    public List<Player> players;
}

public class TeamDetail : MonoBehaviour
{
    public TMPro.TMP_Text displayName;
    public TMPro.TMP_Text teamDisplayName;
    public PastGame pastgame;
    public TeamDetailPlayerCard teamPlayer;
    public GameObject gameContainer;
    public GameObject playerContainer;

    private Dictionary<string, PastGame> gameRowObjectMap;
    private Dictionary<string, TeamDetailPlayerCard> teamDetailObjectMap;

    void Start()
    {
        displayName.text = State.DisplayName;
    }

    void OnEnable()
    {
        if (gameRowObjectMap == null || teamDetailObjectMap == null)
        {
            resetOrInitializeVariables();
        }

        foreach(string key in gameRowObjectMap.Keys)
        {
            Destroy(gameRowObjectMap[key].gameObject);
        }
        foreach(string key in teamDetailObjectMap.Keys)
        {
            Destroy(teamDetailObjectMap[key].gameObject);
        }
        resetOrInitializeVariables();

        StartCoroutine(GetGamesAndPlayers_Coroutine());
    }

    void OnDisable()
    {
        StopCoroutine(GetGamesAndPlayers_Coroutine());
    }

    private void resetOrInitializeVariables()
    {
        gameRowObjectMap = new Dictionary<string, PastGame>();
        teamDetailObjectMap = new Dictionary<string, TeamDetailPlayerCard>();
    }

    IEnumerator GetGamesAndPlayers_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Response response = Client.GetResponseValue(request);
            TeamResponse teamResponse = JsonConvert.DeserializeObject<TeamResponse>(response.data.ToString());
            teamDisplayName.text = teamResponse.name;
            List<Game> pastGames = teamResponse.games;
            List<Player> teamPlayers = teamResponse.players;
            for (int i = 0; i < pastGames.Count; i++)
            {
                Game game = pastGames[i];
                if (pastgame != null && game.id != null)
                {
                    var row = Instantiate(pastgame, gameContainer.transform).GetComponent<PastGame>();
                    row.roomname.text = game.roomName;
                    row.score.text = game.score.ToString();
                    row.minutes.text = game.minutesPlayed.ToString();
                    row.date.text = game.startedAt;
                    row.id = game.id;

                    gameRowObjectMap[game.id] = row;
                }
            }

            for (int i = 0; i < teamPlayers.Count; i++)
            {
                Player player = teamPlayers[i];
                if (teamPlayer != null)
                {
                    TeamDetailPlayerCard row = Instantiate(teamPlayer, playerContainer.transform).GetComponent<TeamDetailPlayerCard>();
                    row.playerName.text = player.displayName;

                    teamDetailObjectMap[player.playerId] = row;
                }
            }
        }
        else Debug.Log(request.result);
    }
}
