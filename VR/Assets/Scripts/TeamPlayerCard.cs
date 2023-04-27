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
    public Dictionary<string, SendingInvitation> onlinePlayersMap;

    void Start()
    {
        if (ScaryVerseState.IsCaptain && !playerId.Equals(ScaryVerseState.PlayerId))
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
        UnityWebRequest request = Client.PrepareRequest("GET", $"/teams/{ScaryVerseState.ActiveTeamId}/delete/player/{playerId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            teamPlayerIds.Remove(playerId);
            onlinePlayersMap.Remove(playerId);
            numberOfPlayersText.text = $"Players ({teamPlayerIds.Count}/4)";

            Destroy(gameObject);
        }
    }
}
