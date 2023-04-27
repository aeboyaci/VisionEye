using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendingInvitationBody
{
    [JsonProperty("playerId")]
    public string playerId;
}

public class SendingInvitation : MonoBehaviour
{
    public string playerId;
    public TMPro.TMP_Text displayName;
    public Button inviteButton;
    public TMPro.TMP_Text pendingText;

    void Start()
    {
        inviteButton.onClick.AddListener(InvitationButtonOnClick);
    }

    private void InvitationButtonOnClick()
    {
        StartCoroutine(SendInvitation_Coroutine());
    }

    IEnumerator SendInvitation_Coroutine()
    {
        UnityWebRequest request = Client.PrepareRequest("POST", $"/teams/{ScaryVerseState.ActiveTeamId}/invitations", new SendingInvitationBody { playerId = this.playerId });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            inviteButton.gameObject.SetActive(false);
            pendingText.gameObject.SetActive(true);
        }
    }
}
