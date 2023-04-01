using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamPlayerCard : MonoBehaviour
{
    public TMPro.TMP_Text displayName;
    public TMPro.TMP_Text captainText;
    public TMPro.TMP_Text numberOfPlayersText;
    public Button kickButton;

    public string playerId;
    public HashSet<string> teamPlayerIds;

    void Start()
    {
        if (State.IsCaptain && !playerId.Equals(State.PlayerId))
        {
            kickButton.gameObject.SetActive(true);
            kickButton.onClick.AddListener(kickButtonOnClick);
        }
    }

    private void kickButtonOnClick()
    {
        StartCoroutine(KickPlayer_Coroutine());
    }

    IEnumerator KickPlayer_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{State.ActiveTeamId}/delete/player/{playerId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            numberOfPlayersText.text = $"Players ({teamPlayerIds.Count - 1}/4)";
        }
    }
}
